using UnityEngine;

public enum ScoreType
{
    Rice, // 米のスコア
    Enemy, // 敵のスコア
    Boss, // ボスのスコア
    Time, // 時間のスコア
    Total   // スコアの種類の数
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

    [Header("スコア設定,0: 米のスコア, 1: 敵のスコア, 2: ボスのスコア, 3: 時間のスコア")]
    [SerializeField] private int[] scoreAmount = new int[(int)ScoreType.Total];

    // 現在のスコア最後の行はTotalスコア用に使用するため、ScoreType.Num + 1のサイズにする
    [SerializeField] private int[] currentScore = new int[(int)ScoreType.Total + 1];

    // --- スコアの管理 ---

    // スコアランキングの配列（上位5位まで）
    [SerializeField] private ScoreData[] scoreRanking = new ScoreData[5];

    public bool isScoreRecorded = false; // スコアが記録されたかどうかのフラグ


    /// <summary>
    /// スコアを加算するメソッド
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(ScoreType type)
    {
        currentScore[(int)type] += scoreAmount[(int)type];
        currentScore[(int)ScoreType.Total] += scoreAmount[(int)type]; // Total scoreも加算
        Debug.Log($"Score added: {scoreAmount[(int)type]}. Total score: {currentScore[(int)ScoreType.Total]}");
    }

    public void AddTimeScore(int time)
    {
        currentScore[(int)ScoreType.Time] += time;
        currentScore[(int)ScoreType.Total] += time; // Total scoreも加算
        Debug.Log($"Time score added: {time}. Total score: {currentScore[(int)ScoreType.Total]}");
    }

    public int[] GetScores()
    {
        return currentScore;
    }


    /// <summary>
    /// スコアを取得するメソッド
    /// </summary>
    /// <returns></returns>
    public int GetTotalScore()
    {
        // Total scoreを返す
        return currentScore[(int)ScoreType.Total];
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
            if (currentScore[(int)ScoreType.Total] > scoreRanking[i].score)
            {
                // スコアを挿入する位置を見つけたら、後ろのスコアをシフトして挿入
                for (int j = scoreRanking.Length - 1; j > i; j--)
                {
                    scoreRanking[j] = scoreRanking[j - 1];
                }
                scoreRanking[i] = new ScoreData(currentScore[(int)ScoreType.Total], "PlayerName");    
                break;
            }
        }
    }

    public void ResetScores()
    {
        for (int i = 0; i < currentScore.Length; i++)
        {
            currentScore[i] = 0;
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