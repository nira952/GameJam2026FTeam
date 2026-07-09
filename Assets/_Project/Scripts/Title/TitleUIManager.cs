using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// タイトル画面のUI管理を行うクラス
public class TitleUIManager : MonoBehaviour
{
    // --- UI参照 ---

    public Button startButton; // 開始ボタン(Managerに公開)
    public Button endButton;   // 終了ボタン(Managerに公開)

    [SerializeField] private Button tutorialButton; // チュートリアルボタン
    [SerializeField] private Button tutorialButotn2;

    [SerializeField] private Button openRankingButton; // ランキングボタン
    [SerializeField] private Button closeRankingButton;

    [SerializeField] private CanvasGroup rankingPanel; // ランキングパネル

    [SerializeField] private CanvasGroup tutorialPanel1;
    [SerializeField] private CanvasGroup tutorialPanel2;

    // ランキングは5位まで表示する
    [SerializeField] private TextMeshProUGUI[] rankingTexts = new TextMeshProUGUI[5];

    [SerializeField] private int[] demoScores = new int[5];

    private void Awake()
    {
        // ランキングボタンのイベントにOpenRankingPanelメソッドを登録
        openRankingButton.onClick.AddListener(OpenRankingPanel);

        // 閉じるボタンのイベントにCloseRankingPanelメソッドを登録
        closeRankingButton.onClick.AddListener(CloseRankingPanel);

        tutorialButton.onClick.AddListener(OnTutorialButton);

        tutorialButotn2.onClick.AddListener(OnTutorialButton2);

    }

    private void Start()
    {
        // ランキングパネルを非表示にする
        rankingPanel.alpha = 0;
        rankingPanel.interactable = false;
        rankingPanel.blocksRaycasts = false;

        tutorialPanel1.alpha = 0;
        tutorialPanel1.interactable = false;
        tutorialPanel1.blocksRaycasts = false;

        tutorialPanel2.alpha = 0;
        tutorialPanel2.interactable = false;
        tutorialPanel2.blocksRaycasts = false;

        // ランキングを更新する
        UpdateScores();
    }

    // ランキングを更新するメソッド
    private void UpdateScores()
    {
        if (ScoreManager.Instance != null)
        {
            if (!ScoreManager.Instance.isScoreRecorded)
            {
                // スコアが記録されていない場合は、仮のスコアを表示する
                for (int i = 0; i < rankingTexts.Length; i++)
                {
                    rankingTexts[i].text = $"{i + 1}. ---: {demoScores[i]}";
                }
                return;
            }

            // スコアが記録されている場合は、実際のランキングを表示する
            var ranking = ScoreManager.Instance.GetScoreRanking();
            for (int i = 0; i < ranking.Length; i++)
            {
                rankingTexts[i].text = $"{i + 1}. {ranking[i].playerName}: {ranking[i].score}";
            }
        }

    }

    // ランキングパネルをフェードで開くメソッド
    private void OpenRankingPanel()
    {
        // ランキングパネルを表示する
        rankingPanel.DOFade(1, 0.5f);
        rankingPanel.interactable = true;
        rankingPanel.blocksRaycasts = true;
 
    }

    // ランキングパネルをフェードで閉じるメソッド
    private void CloseRankingPanel()
    {
        // ランキングパネルを非表示にする
        rankingPanel.DOFade(0, 0.5f);
        rankingPanel.interactable = false;
        rankingPanel.blocksRaycasts = false;
    }


    private void OnTutorialButton()
    {
        // チュートリアルパネルを表示する
        tutorialPanel1.DOFade(1, 0.5f);
        tutorialPanel1.interactable = true;
        tutorialPanel1.blocksRaycasts = true;
    }

    private void OnTutorialButton2()
    {
        // チュートリアルパネルを非表示する
        tutorialPanel1.DOFade(0, 0.5f);
        tutorialPanel1.interactable = false;
        tutorialPanel1.blocksRaycasts = false;

        // チュートリアルパネルを表示する
        tutorialPanel2.DOFade(1, 0.5f);
        tutorialPanel2.interactable = true;
        tutorialPanel2.blocksRaycasts = true;
    }


}
