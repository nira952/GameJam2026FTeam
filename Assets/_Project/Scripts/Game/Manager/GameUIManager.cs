using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Button endGameButton; // ゲーム終了ボタン(Managerに公開)
    public Button retryButton;   // リトライボタン(Managerに公開)

    [SerializeField] private TextMeshProUGUI countDownText;

    [SerializeField] private TextMeshProUGUI timerText; // タイマー表示用のテキスト

    [SerializeField] private CanvasGroup resultPanel; // リザルトパネル
    [SerializeField] private TextMeshProUGUI[] resultScoreText = new TextMeshProUGUI[(int)ScoreType.Total]; // リザルトスコアテキスト


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

    public void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }


    /// <summary>
    /// リザルトパネルを開く
    /// </summary>
    public void OpenResultPanel(int[] scores)
    {
        for (int i = 0; i < scores.Length && i < resultScoreText.Length; i++)
        {
            string scoreText = $"{(ScoreType)i}: {scores[i]}";

            resultScoreText[i].text = scoreText;
        }
    
        resultPanel.DOFade(1, 0.5f);
        resultPanel.interactable = true;
        resultPanel.blocksRaycasts = true;
    }
}
