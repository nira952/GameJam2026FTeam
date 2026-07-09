using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public enum WaveDirection
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}

public class SpawnManager : MonoBehaviour
{
    // --- シングルトンパターンの実装 ---
    #region Singleton
    public static SpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion


    // --- コールバックイベント ---
    /// <summary>
    /// BossEnemyが生成された瞬間に呼ばれるコールバック
    /// </summary>
    public event Action OnBossSpawned;

    /// <summary>
    /// 波（ウェーブ）が切り替わった瞬間に呼ばれるコールバック（引数: 新しい注目方向）
    /// </summary>
    public event Action<WaveDirection> OnWaveChanged;


    [Header("--- 必須設定（敵プレハブ） ---")]
    [SerializeField] private BaseEnemy defaultEnemyPrefab;
    [SerializeField] private BaseEnemy zigZagEnemyPrefab;
    [SerializeField] private BaseEnemy bossEnemyPrefab;

    [Space(10)]
    [Tooltip("ゲームの中心となるオブジェクト")]
    [SerializeField] private Transform fenceObject;

    [Tooltip("生成された敵を格納する親オブジェクト")]
    [SerializeField] private Transform poolParent;

    [Header("--- パラメータ設定 ---")]
    [Tooltip("スポーン系パラメータSO")]
    [SerializeField] private SpawnSettingsData spawnSettings;

    [Header("--- メモリ管理（オブジェクトプール） ---")]
    [Min(1)]
    [Tooltip("ゲーム開始時にあらかじめ用意しておく各敵の数")]
    [SerializeField] private int defaultCapacity = 20;

    [Min(1)]
    [Tooltip("各プール内に溜めておける敵の最大数")]
    [SerializeField] private int maxPoolSize = 50;

    // --- 内部処理用変数 ---
    private IObjectPool<BaseEnemy> defaultEnemyPool;
    private IObjectPool<BaseEnemy> zigZagEnemyPool;
    private IObjectPool<BaseEnemy> bossEnemyPool;

    private float defaultEnemyTimer;
    private float zigZagEnemyTimer;
    private float bossEnemyTimer;

    private float currentSpawnMultiplier = 1.0f;
    private float elapsedTime;

    private WaveDirection currentFavoriteDirection;
    private float waveDuration;
    private float waveTimer;

    private readonly List<BaseEnemy> activeEnemies = new List<BaseEnemy>();

    public IReadOnlyList<BaseEnemy> ActiveEnemies => activeEnemies;

    public Vector3 GetRandomActiveEnemiePos()
    {
        if (activeEnemies.Count == 0) return Vector3.zero;
        int randomIndex = UnityEngine.Random.Range(0, activeEnemies.Count);
        return activeEnemies[randomIndex].transform.position;
    }

    private void Start()
    {
        if (spawnSettings == null)
        {
            Debug.LogError($"{gameObject.name}: SpawnSettingsData がインスペクターにセットされていません！");
            enabled = false;
            return;
        }

        if (poolParent == null) poolParent = new GameObject("EnemyPool").transform;

        defaultEnemyPool = CreateEnemyPool(defaultEnemyPrefab);
        zigZagEnemyPool = CreateEnemyPool(zigZagEnemyPrefab);
        bossEnemyPool = CreateEnemyPool(bossEnemyPrefab);

        currentSpawnMultiplier = spawnSettings.initialSpawnInterval;

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

        // 第4引数（isBoss）を BossEnemy の時だけ true にする
        HandleSpawning(ref defaultEnemyTimer, spawnSettings.defaultEnemyInterval, defaultEnemyPool, false);
        HandleSpawning(ref zigZagEnemyTimer, spawnSettings.zigZagEnemyInterval, zigZagEnemyPool, false);
        HandleSpawning(ref bossEnemyTimer, spawnSettings.bossEnemyInterval, bossEnemyPool, true);
    }

    private void UpdateSpawnSpeed()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / spawnSettings.timeToReachMaxSpeed);

        float minMultiplier = Mathf.Min(spawnSettings.initialSpawnInterval, spawnSettings.minSpawnInterval);
        currentSpawnMultiplier = Mathf.Lerp(spawnSettings.initialSpawnInterval, minMultiplier, progress);
    }

    /// <summary>
    /// 指定された敵のタイマーを更新し、間隔を満たせばスポーンさせる
    /// </summary>
    private void HandleSpawning(ref float timer, float baseInterval, IObjectPool<BaseEnemy> pool, bool isBoss)
    {
        timer += Time.deltaTime;
        float currentInterval = baseInterval * currentSpawnMultiplier;

        if (timer >= currentInterval)
        {
            SpawnEnemy(pool, isBoss);
            timer = 0f;
        }
    }

    private void SetupNextWave()
    {
        currentFavoriteDirection = (WaveDirection)UnityEngine.Random.Range(0, 4);

        float minWave = spawnSettings.minWaveDuration;
        float maxWave = Mathf.Max(spawnSettings.minWaveDuration, spawnSettings.maxWaveDuration);
        waveDuration = UnityEngine.Random.Range(minWave, maxWave);

        waveTimer = 0f;

        // 波切り替えコールバックの実行（登録されたメソッドがあれば通知する）
        OnWaveChanged?.Invoke(currentFavoriteDirection);

        string[] dirNames = { "上", "下", "左", "右" };
        Debug.Log($"【波の切り替え】次の {waveDuration:F1} 秒間は「{dirNames[(int)currentFavoriteDirection]}」から！ (現在の倍率: x{currentSpawnMultiplier:F2})");
    }

    /// <summary>
    /// 指定されたプールから敵を取り出してスポーンさせる
    /// </summary>
    private void SpawnEnemy(IObjectPool<BaseEnemy> pool, bool isBoss)
    {
        BaseEnemy enemy = pool.Get();

        if (isBoss)
        {
            // ボスの場合は方向を「Up（上）」に強制指定して位置計算を行う
            enemy.transform.position = CalculateSpawnPosition(WaveDirection.Up);

            // ボス生成コールバックの実行
            OnBossSpawned?.Invoke();
        }
        else
        {
            // 通常の敵はウェーブ等のランダム計算に従う
            enemy.transform.position = CalculateSpawnPosition(null);
        }
    }

    /// <summary>
    /// スポーン位置を計算する。overrideDirectionが指定されている場合はその方向を固定で使用する。
    /// </summary>
    private Vector3 CalculateSpawnPosition(WaveDirection? overrideDirection)
    {
        WaveDirection finalDirection;

        // 特殊な方向指定（ボスの「上」など）があればそれを使い、なければ通常計算
        if (overrideDirection.HasValue)
        {
            finalDirection = overrideDirection.Value;
        }
        else
        {
            if (UnityEngine.Random.value < spawnSettings.favoriteDirectionChance)
            {
                finalDirection = currentFavoriteDirection;
            }
            else
            {
                WaveDirection[] otherDirections = new WaveDirection[3];
                int index = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (i != (int)currentFavoriteDirection)
                    {
                        otherDirections[index] = (WaveDirection)i;
                        index++;
                    }
                }
                finalDirection = otherDirections[UnityEngine.Random.Range(0, 3)];
            }
        }

        float variance = UnityEngine.Random.Range(-spawnSettings.positionVariance, spawnSettings.positionVariance);
        Vector3 spawnPos = Vector3.zero;

        switch (finalDirection)
        {
            case WaveDirection.Up:
                spawnPos = new Vector3(variance, spawnSettings.spawnDistance, 0f);
                break;
            case WaveDirection.Down:
                spawnPos = new Vector3(variance, -spawnSettings.spawnDistance, 0f);
                break;
            case WaveDirection.Left:
                spawnPos = new Vector3(-spawnSettings.spawnDistance, variance, 0f);
                break;
            case WaveDirection.Right:
                spawnPos = new Vector3(spawnSettings.spawnDistance, variance, 0f);
                break;
        }

        if (fenceObject != null)
        {
            spawnPos += fenceObject.position;
        }

        return spawnPos;
    }

    private IObjectPool<BaseEnemy> CreateEnemyPool(BaseEnemy prefab)
    {
        IObjectPool<BaseEnemy> pool = null;

        pool = new ObjectPool<BaseEnemy>(
            createFunc: () =>
            {
                BaseEnemy enemy = Instantiate(prefab, poolParent);
                enemy.SetPool(pool);
                enemy.SetTarget(fenceObject);
                return enemy;
            },
            actionOnGet: OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize
        );

        return pool;
    }

    private void OnGetEnemy(BaseEnemy enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.OnSpawn();

        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    private void OnReleaseEnemy(BaseEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        activeEnemies.Remove(enemy);
    }

    private void OnDestroyEnemy(BaseEnemy enemy)
    {
        activeEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}