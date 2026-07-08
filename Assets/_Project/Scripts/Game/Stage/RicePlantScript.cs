using UnityEngine;

public class RicePlantScript : MonoBehaviour
{
    private enum GrowthStage
    {
        Brown = 0,      // 段階0: 茶色（植えたて / 収穫後）
        Green = 1,      // 段階1: 緑
        LightGreen = 2, // 段階2: 黄緑
        Yellow = 3      // 段階3: 黄色（収穫可能！）
    }

    [Header("--- 成長スピード設定 ---")]
    [Tooltip("1段階成長するのに必要な時間（秒）")]
    [SerializeField] private float timePerStage = 5f;

    [Header("--- 見た目の設定 (Sprite) ---")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] growthSprites = new Sprite[4]; // 最初から要素数4で初期化

    [SerializeField] private Color[] growthColors = new Color[4]; // 成長段階に応じた色を設定する配列

    [SerializeField] private GrowthStage currentStage = GrowthStage.Brown;
    private float growthTimer;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        DebugUpdateAppearance();
    }

    private void Update()
    {
        // ゲームがプレイ中でなければ成長処理を行わない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) return;

        // すでに黄色（最大）なら成長タイマーは進めない
        if (currentStage == GrowthStage.Yellow) return;

        growthTimer += Time.deltaTime;
        if (growthTimer >= timePerStage)
        {
            GrowNextStage();
            growthTimer = 0f;
        }
    }

    // 次の段階へ成長
    private void GrowNextStage()
    {
        if (currentStage < GrowthStage.Yellow)
        {
            currentStage++;
            DebugUpdateAppearance();
        }
    }


    private void DebugUpdateAppearance()
    {
        if (growthColors == null || growthColors.Length == 0) return;

       
        int index = (int)currentStage;


        if (index < growthColors.Length)
        {
            spriteRenderer.color = growthColors[index];
        }
            else
            {
                Debug.LogWarning($"{gameObject.name}: 成長段階 {currentStage} のColorが空っぽです！");
            }
        }
    


    // 段階に応じて画像を切り替える
    private void UpdateAppearance()
    {
        if (growthSprites == null || growthSprites.Length == 0) return;

        // 現在のステージ番号（0〜3）をそのまま配列のインデックスとして使用
        int index = (int)currentStage;

        // プランナーさんがインスペクターで画像を入れ忘れたり、
        // 配列の数を変えてしまっていた場合の安全ガード
        if (index < growthSprites.Length)
        {
            if (growthSprites[index] != null)
            {
                spriteRenderer.sprite = growthSprites[index];
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: 成長段階 {currentStage} のSpriteが空っぽです！");
            }
        }
    }

    // プレイヤーから呼ばれる「収穫してくれ！」という関数
    public bool Harvest()
    {
        // 黄色のときだけ収穫を許可する
        if (currentStage != GrowthStage.Yellow) { return false; }


        Debug.Log("稲を収穫しました！");

        // 最初の段階（茶色）に戻る
        currentStage = GrowthStage.Brown;
        growthTimer = 0f;
        DebugUpdateAppearance();

        MegaTaikoScript.Instance.AddRicePower();

        // スコアを加算する
        ScoreManager.Instance.AddScore(ScoreType.Rice);

        return true;
        


    }
}