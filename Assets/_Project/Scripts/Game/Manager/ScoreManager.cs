using UnityEngine;

public enum ScoreType
{
    Rice, // 米のスコア
    Enemy, // 敵のスコア
    Boss, // ボスのスコア
    Num   // スコアの種類の数
}


public struct ScoreData
{
    public int score;
    public string playerName;
    public ScoreData(int score, string playerName)
    {
        this.score = score;
        this.playerName = playerName;
    }
}

public class ScoreManager : MonoBehaviour
{
    // --- シングルトンパターンの実装 ---

    #region Singleton
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        // 1. すでに他のシーン等で同じマネージャーが作られていた場合
        if (Instance != null && Instance != this)
        {
            // 重複して作られた方を即座に削除して、古い方を残す
            Destroy(gameObject);
            return;
        }

        // 2. 自分が最初のインスタンスなら、Instanceに登録
        Instance = this;

        // 3. シーンが切り替わっても、このオブジェクトを破壊しないように設定
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private int[] scoreAmount = new int[(int)ScoreType.Num];


    // --- スコアの管理 ---
    [SerializeField] private int score = 0; // スコアの初期値

    // スコアランキングの配列（上位5位まで）
    [SerializeField] private ScoreData[] scoreRanking = new ScoreData[5];

    public bool isScoreRecorded = false; // スコアが記録されたかどうかのフラグ


    /// <summary>
    /// スコアを加算するメソッド
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(ScoreType type)
    {
        score += scoreAmount[(int)type];
        Debug.Log($"Score added: {scoreAmount[(int)type]}. Total score: {score}");
    }

    /// <summary>
    /// スコアを取得するメソッド
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return score;
    }   

    public ScoreData[] GetScoreRanking()
    {
        return scoreRanking;
    }

    public void RecordScore()
    {
        // スコア記録済みのフラグを立てる
        if (isScoreRecorded == false) { isScoreRecorded = true; }

        // 現在のスコアをランキングに追加
        for (int i = 0; i < scoreRanking.Length; i++)
        {
            if (score > scoreRanking[i].score)
            {
                // スコアを挿入する位置を見つけたら、後ろのスコアをシフトして挿入
                for (int j = scoreRanking.Length - 1; j > i; j--)
                {
                    scoreRanking[j] = scoreRanking[j - 1];
                }
                scoreRanking[i] = new ScoreData(score, "PlayerName"); 
                break;
            }
        }
    }

    private void OnDestroy()
    {
        // シングルトンのインスタンスを破棄する際に、Instanceをnullに設定
        if (Instance == this)
        {
            Instance = null;
        }
    }

}