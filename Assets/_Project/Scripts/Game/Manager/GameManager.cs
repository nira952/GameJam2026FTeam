using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,   // ゲーム開始前の状態
    Playing, // ゲームプレイ中の状態
    Paused,  // ゲーム一時停止の状態
    GameOver // ゲームオーバーの状態
}



public class GameManager : MonoBehaviour
{
    // --- シングルトンパターンの実装 ---
    #region Singleton
    public static GameManager Instance { get; private set; }

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

    // --- クラス参照 ---
    [SerializeField] private GameUIManager gameUIManager; // GameUIManagerの参照


    // --- ゲーム状態の管理 ---
    private GameState currentGameState = GameState.Ready; // 初期状態はReady

    // ゲーム状態の公開プロパティ
    public GameState CurrentGameState => currentGameState;


    // ゲーム状態が変化したときに呼ばれるイベント
    private void OnGameStateChanged(GameState newState)
    {
        // ゲーム状態が変化したときの処理をここに追加できます
        Debug.Log($"Game State changed to: {newState}");
    }

    private void ButtonAddListener()
    {
        gameUIManager.endGameButton.onClick.AddListener(EndGame);
        gameUIManager.retryButton.onClick.AddListener(RetryGame);
    }


    private void Start()
    {
        // ボタンのリスナーを追加
        ButtonAddListener();

        // 開始時はReady状態に設定
        currentGameState = GameState.Ready;

        // ゲーム開始のカウントダウンを開始
        StartCoroutine(StartGame());
    }

    // ゲーム開始処理
    private IEnumerator StartGame()
    {
        // 3秒のカウントダウンを行う
        for (int i = 3; i > 0; i--)
        {
            Debug.Log(i);
            gameUIManager.UpdateCountDownText(i);
            yield return new WaitForSeconds(1f);
        }

        gameUIManager.UpdateCountDownText(0); // カウントダウン終了後に0を表示
        // カウントダウン終了後、ゲーム状態をPlayingに変更
        currentGameState = GameState.Playing;
    }

    /// <summary>
    /// ゲームの一時停止
    /// </summary>
    public void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            currentGameState = GameState.Paused;
            Time.timeScale = 0f; // ゲームを一時停止
            OnGameStateChanged(currentGameState);
        }
    }

    /// <summary>
    /// 一時停止を解除
    /// </summary>
    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            currentGameState = GameState.Playing;
            Time.timeScale = 1f; // ゲームを再開
            OnGameStateChanged(currentGameState);
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        Debug.Log("GameOver!");

        currentGameState = GameState.GameOver;
        OnGameStateChanged(currentGameState);

        if (ScoreManager.Instance != null)
        {
            // スコアを記録する処理を呼び出す
            ScoreManager.Instance.RecordScore();
        }

        // リザルトパネルを開く
        gameUIManager.OpenResultPanel(ScoreManager.Instance.GetScore());
    }

    private void RetryGame()
    {
        // ゲームオーバー後にリトライする場合、ゲーム状態をReadyに戻す
        currentGameState = GameState.Ready;
        OnGameStateChanged(currentGameState);
        // ゲームシーンを再読み込みしてリトライ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndGame()
    {
        SceneManager.LoadScene("TitleScene"); // タイトルシーンに戻る
    }

    private void OnDestroy()
    {
        // シングルトンのインスタンスを破棄する際にnullに設定
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
