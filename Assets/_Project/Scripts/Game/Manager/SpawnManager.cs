using System.Collections.Generic; // 必須：Listを使うために追加
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    // --- シングルトンパターンの実装 ---
    #region Singleton
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        // すでにインスタンスが存在しているかチェック
        if (Instance != null && Instance != this)
        {
            // 重複している場合は自身を破棄
            Destroy(gameObject);
            return;
        }

        // インスタンスを自身に設定
        Instance = this;
    }
    #endregion


    private enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    [Header("--- 必須設定 ---")]
    [Tooltip("出現させる敵のプレハブ")]
    [SerializeField] private BaseEnemy enemyPrefab;

    [Tooltip("ゲームの中心となるオブジェクト")]
    [SerializeField] private Transform fenceObject;

    [Tooltip("生成された敵を格納する親オブジェクト")]
    [SerializeField] private Transform poolParent;

    [Header("--- パラメータ設定 ---")]
    [Tooltip("スポーン系パラメータSO")]
    [SerializeField] private SpawnSettingsData spawnSettings;

    [Header("--- メモリ管理（オブジェクトプール） ---")]
    [Min(1)]
    [Tooltip("ゲーム開始時にあらかじめメモリ上に用意しておく敵の数")]
    [SerializeField] private int defaultCapacity = 20;

    [Min(1)]
    [Tooltip("プール内に溜めておける敵の最大数")]
    [SerializeField] private int maxPoolSize = 50;

    // --- 内部処理用変数 ---
    private IObjectPool<BaseEnemy> enemyPool;
    private float spawnTimer;
    private float currentSpawnInterval;
    private float elapsedTime;

    private Direction currentFavoriteDirection;
    private float waveDuration;
    private float waveTimer;

    // --- 稼働中の敵を管理するリスト ---
    private readonly List<BaseEnemy> activeEnemies = new List<BaseEnemy>();

    /// <summary>
    /// 現在稼働している敵の読み取り専用リスト（外部参照用）
    /// </summary>
    public IReadOnlyList<BaseEnemy> ActiveEnemies => activeEnemies;

    public Vector3 GetRandomActiveEnemiePos()
    {
        // 稼働中の敵がいない場合は Vector3.zero を返す
        if (activeEnemies.Count == 0)
        {
            return Vector3.zero;
        }

        // ランダムに敵を選択してその位置を返す
        int randomIndex = Random.Range(0, activeEnemies.Count);

        return activeEnemies[randomIndex].transform.position;
    }


    private void Start()
    {
        // SOがセットされていなかったらエラーを出す
        if (spawnSettings == null)
        {
            Debug.LogError($"{gameObject.name}: SpawnSettingsData がインスペクターにセットされていません！");
            enabled = false;
            return;
        }

        if (poolParent == null)
        {
            poolParent = new GameObject("EnemyPool").transform;
        }

        if (spawnSettings.minSpawnInterval > spawnSettings.initialSpawnInterval)
        {
            Debug.LogWarning("SpawnManager: 最大難易度の間隔が初期間隔より大きく設定されていました。自動的に制限をかけます。");
        }

        enemyPool = new ObjectPool<BaseEnemy>(
            createFunc: OnCreateEnemy,
            actionOnGet: OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        currentSpawnInterval = spawnSettings.initialSpawnInterval;

        SetupNextWave();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

        UpdateSpawnSpeed();

        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            SetupNextWave();
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void UpdateSpawnSpeed()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / spawnSettings.timeToReachMaxSpeed);

        float minInterval = Mathf.Min(spawnSettings.initialSpawnInterval, spawnSettings.minSpawnInterval);
        currentSpawnInterval = Mathf.Lerp(spawnSettings.initialSpawnInterval, minInterval, progress);
    }

    private void SetupNextWave()
    {
        currentFavoriteDirection = (Direction)Random.Range(0, 4);

        float minWave = spawnSettings.minWaveDuration;
        float maxWave = Mathf.Max(spawnSettings.minWaveDuration, spawnSettings.maxWaveDuration);
        waveDuration = Random.Range(minWave, maxWave);

        waveTimer = 0f;

        string[] dirNames = { "上", "下", "左", "右" };
        Debug.Log($"【波の切り替え】次の {waveDuration:F1} 秒間は「{dirNames[(int)currentFavoriteDirection]}」から！ (現在の間隔: {currentSpawnInterval:F2}秒)");
    }

    private void SpawnEnemy()
    {
        BaseEnemy enemy = enemyPool.Get();
        enemy.transform.position = CalculateSpawnPosition();
    }

    private Vector3 CalculateSpawnPosition()
    {
        Direction finalDirection;
        if (Random.value < spawnSettings.favoriteDirectionChance)
        {
            finalDirection = currentFavoriteDirection;
        }
        else
        {
            Direction[] otherDirections = new Direction[3];
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i != (int)currentFavoriteDirection)
                {
                    otherDirections[index] = (Direction)i;
                    index++;
                }
            }
            finalDirection = otherDirections[Random.Range(0, 3)];
        }

        float variance = Random.Range(-spawnSettings.positionVariance, spawnSettings.positionVariance);
        Vector3 spawnPos = Vector3.zero;

        switch (finalDirection)
        {
            case Direction.Up:
                spawnPos = new Vector3(variance, spawnSettings.spawnDistance, 0f);
                break;
            case Direction.Down:
                spawnPos = new Vector3(variance, -spawnSettings.spawnDistance, 0f);
                break;
            case Direction.Left:
                spawnPos = new Vector3(-spawnSettings.spawnDistance, variance, 0f);
                break;
            case Direction.Right:
                spawnPos = new Vector3(spawnSettings.spawnDistance, variance, 0f);
                break;
        }

        if (fenceObject != null)
        {
            spawnPos += fenceObject.position;
        }

        return spawnPos;
    }

    private BaseEnemy OnCreateEnemy()
    {
        BaseEnemy enemy = Instantiate(enemyPrefab, poolParent);
        enemy.SetPool(enemyPool);
        enemy.SetTarget(fenceObject);

        return enemy;
    }

    private void OnGetEnemy(BaseEnemy enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.OnSpawn();

        // リストに追加
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    private void OnReleaseEnemy(BaseEnemy enemy)
    {
        enemy.gameObject.SetActive(false);

        // リストから削除
        activeEnemies.Remove(enemy);
    }

    private void OnDestroyEnemy(BaseEnemy enemy)
    {
        // 万が一、アクティブなまま破棄された場合の安全策
        activeEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}