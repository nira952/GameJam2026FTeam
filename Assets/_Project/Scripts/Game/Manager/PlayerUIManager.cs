using UnityEngine;
using UnityEngine.UI;

// プレイヤーのUI管理を担当するスクリプト
public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Slider ricePowerSlider; // 米の力スライダー


    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="maxRicePower"></param>
    public void Initialize(float maxRicePower,float currentRicePower)
    {
       SettingMaxRicePower(maxRicePower,currentRicePower);

    }

    // スライダーの最大値を設定する関数
    private void SettingMaxRicePower(float power,float currentPower)
    {
        ricePowerSlider.maxValue = power;      // スライダーの最大値を設定
        ricePowerSlider.value = currentPower;  // スライダーの初期値を設定

    }

    /// <summary>
    /// スライダーの値を更新する関数
    /// </summary>
    /// <param name="value"></param>
    public void UpdateRicePowerSlider(float value)
    {
        ricePowerSlider.value = value;

    }

}
