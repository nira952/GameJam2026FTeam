using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MegaTaikoUIScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup megaTaikoCanvasGroup; // メガ太鼓モードのUIを管理するCanvasGroup

    [SerializeField] private Slider ricePowerSlider; // 稲パワーのスライダーUI

    [SerializeField] private Image riceSliderFill;

    [SerializeField] private Gradient colorGradient;

    [Header("演出設定")]
    [SerializeField] private Color errorColor = Color.red; // 失敗時の色
    [SerializeField] private float fadeDuration = 0.5f;    // 元に戻る時間（秒）

    private Tweener colorTweener; // 再生中のアニメーションを管理する変数

    private void Start()
    {
        megaTaikoCanvasGroup.alpha = 0f; // 初期状態では非表示にする

        // 初期カラーを設定
        UpdateSliderColor(ricePowerSlider.value);

        // スライダーの値が変更されたときに実行するイベントを登録
        ricePowerSlider.onValueChanged.AddListener(UpdateSliderColor);
    }

    /// <summary>
    /// スライダーの最大値を設定します。
    /// </summary>
    /// <param name="maxValue"></param>
    public void SettingRiceSlider(float maxValue)
    {
        ricePowerSlider.maxValue = maxValue;
        ricePowerSlider.value = 0f; // 初期値を0に設定
    }

    public void UpdateRiceSlider(float currentValue)
    {
        ricePowerSlider.value = currentValue;

        OpenMegaTaikoPanel(); // スライダー更新時にパネルを開く
    }

    // 現在のスライダーの値に応じたグラデーション色を適用する共通メソッド
    private void ApplyCurrentGradientColor()
    {
        float normalizedValue = ricePowerSlider.normalizedValue;
        riceSliderFill.color = colorGradient.Evaluate(normalizedValue);
    }


    void UpdateSliderColor(float value)
    {
        // Sliderの「現在の値 / 最大値」で0~1の割合（Normalized Value）を計算
        // ※SliderのMinが0、Maxが1の場合はそのまま value でOKです
        float normalizedValue = ricePowerSlider.normalizedValue;

        // グラデーションから色を抽出してFill Imageに適用
        riceSliderFill.color = colorGradient.Evaluate(normalizedValue);
    }

    /// <summary>
    /// 外部から呼び出す「使用失敗」の演出メソッド
    /// </summary>
    public void PlayErrorAnimation()
    {
        if (riceSliderFill == null) return;

        // すでに再生中のアニメーションがあれば安全に破棄する
        colorTweener?.Kill();

        // 1. 瞬時に赤色（errorColor）に変える
        riceSliderFill.color = errorColor;

        // 2. 現在のスライダー位置の「本来のグラデーション色」を計算
        float normalizedValue = ricePowerSlider.normalizedValue;
        Color targetColor = colorGradient.Evaluate(normalizedValue);

        // 3. DOTweenで元の色へフェード
        colorTweener = riceSliderFill.DOColor(targetColor, fadeDuration)
            .SetEase(Ease.OutCubic) // 滑らかなイージング（お好みで変更してください）
            .OnComplete(() =>
            {
                // 完了時に一応、最新のグラデーション色を再適用して整合性を保つ
                ApplyCurrentGradientColor();
            });
    }


    public void OpenMegaTaikoPanel()
    {
        megaTaikoCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    public void CloseMegaTaikoPanel()
    {
        megaTaikoCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
    }

    void OnDestroy()
    {
        // リスナーの解除
        if (ricePowerSlider != null)
        {
            ricePowerSlider.onValueChanged.RemoveListener(UpdateSliderColor);
        }

        colorTweener?.Kill();
    }

}
