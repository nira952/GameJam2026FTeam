using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Button endGameButton; // ゲーム終了ボタン(Managerに公開)
    public Button retryButton;   // リトライボタン(Managerに公開)

    [SerializeField] private TextMeshProUGUI countDownText;

    [SerializeField] private CanvasGroup resultPanel; // リザルトパネル
    [SerializeField] private TextMeshProUGUI resultScoreText; // リザルトスコアテキスト


    /// <summary>
    /// カウントダウンのテキストを更新
    /// </summary>
    /// <param name="count"></param>
    public void UpdateCountDownText(int count)
    {
        // カウントが0になったら"GO!"と表示し、フェードアウトさせる
        if (count == 0)
        {
            countDownText.text = "GO!";
            countDownText.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
            {
                countDownText.text = "";
            });

            return;
        }

        countDownText.text = count.ToString();
    }


    /// <summary>
    /// リザルトパネルを開く
    /// </summary>
    public void OpenResultPanel(int score)
    {
        resultScoreText.text = $"Score: {score}";
        resultPanel.DOFade(1, 0.5f);
        resultPanel.interactable = true;
        resultPanel.blocksRaycasts = true;
    }
}
