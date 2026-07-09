using UnityEngine;
using UnityEngine.SceneManagement;

// タイトル画面の管理を行うクラス
public class TitleManager : MonoBehaviour
{
    [SerializeField] private TitleUIManager titleUIManager;
    // 移動先のシーン名
    [SerializeField] private string nextSceneName = "GameScene";


    private void Start()
    {
        // TitleUIManagerの参照を取得
        if (titleUIManager == null)
        {
            Debug.LogWarning("TitleUIManagerがアタッチされていません");
            return;
        }

        AudioManager.Instance.Play(BgmName.Title);

        // ボタンのリスナーを追加
        ButtonAddListener();
    }

    private void ButtonAddListener()
    {
        // StartボタンのイベントにStartGameメソッドを登録
        titleUIManager.startButton.onClick.AddListener(StartGame);

        titleUIManager.endButton.onClick.AddListener(EndGame);

    }



    public void StartGame()
    {
        // 指定されたシーンに遷移する
        SceneManager.LoadScene(nextSceneName);
    }

    public void EndGame()
    {
        // UnityEditor上での終了処理
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        // ゲームを終了する
        Application.Quit();
    }
}
