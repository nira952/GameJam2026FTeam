using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Button endGameButton; // ゲーム終了ボタン(Managerに公開)
    public Button retryButton;   // リトライボタン(Managerに公開)

    [SerializeField] private TextMeshProUGUI countDownText;

    [SerializeField] private TextMeshProUGUI timerText; // タイマー表示用のテキスト

    [SerializeField] private TextMeshProUGUI finishText; // ゲーム終了時のテキスト

    [SerializeField] private CanvasGroup resultPanel; // リザルトパネル
    [SerializeField] private TextMeshProUGUI[] resultScoreText = new TextMeshProUGUI[(int)ScoreType.Total]; // リザルトスコアテキスト


    [SerializeField] private Image bossImage;

    [Header("警告表示 0:上 1:下 2:左 3:右")]
    [SerializeField] private Image[] warningImages = new Image[4]; // 警告画像の配列


    private void Start()
    {
        // リザルトパネルを非表示にする
        resultPanel.alpha = 0;
        resultPanel.interactable = false;
        resultPanel.blocksRaycasts = false;

        if(SpawnManager.Instance != null)
        {
            SpawnManager.Instance.OnBossSpawned += OnBossSpawned;
            SpawnManager.Instance.OnWaveChanged += OnWaveChanged;

        }
    }



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


    public void ShowFinishText()
    {
        finishText.DOFade(1, 0.5f).OnComplete(() =>
        {
            finishText.DOFade(0, 0.5f).SetDelay(1f); ;
        });

    }


    /// <summary>
    /// リザルトパネルを開く
    /// </summary>
    public IEnumerator OpenResultPanel(int[] scores)
    {

        for (int i = 0; i < scores.Length && i < resultScoreText.Length; i++)
        {

            string scoreText = $"{(ScoreType)i}: 0000";

            resultScoreText[i].text = scoreText;
        }


        resultPanel.DOFade(1, 0.5f);


        yield return new WaitForSeconds(2.0f);


        for (int i = 0; i < scores.Length && i < resultScoreText.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);

            string scoreText = $"{(ScoreType)i}: {scores[i]}";

            resultScoreText[i].text = scoreText;

            AudioManager.Instance.Play(SeName.Don);
        }

        yield return new WaitForSeconds(0.1f);

        AudioManager.Instance.Play(SeName.Don);

        resultPanel.interactable = true;
        resultPanel.blocksRaycasts = true;
    }



    private void OnBossSpawned()
    {

        bossImage.DOFade(1, 0.5f).SetLoops(6, LoopType.Yoyo);

    }

    private void OnWaveChanged(WaveDirection direction)
    {
         // 警告表示を更新
         // イメージを二回フェードさせる
         int index = (int)direction;

        warningImages[index].DOFade(1, 0.5f).SetLoops(4, LoopType.Yoyo);

        AudioManager.Instance.Play(SeName.Alert);

    }


}
