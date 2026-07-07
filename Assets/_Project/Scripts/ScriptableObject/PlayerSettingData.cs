using UnityEngine;
namespace Rina_Script
{

    // ScriptableObjectとしてプレイヤーの設定データを管理するクラス
    [CreateAssetMenu(fileName = "PlayerSettingData", menuName = "ScriptableObjects/PlayerSettingData")]
    public class PlayerSettingData : ScriptableObject
    {
        [Header("稲力の最大値")]
        public float maxRicePower = 10f;            // 稲力の最大値

        [Header("稲力の初期値")]
        public float currentRicePower = 5f;         // 稲力の初期値

        [Header("一回の収穫で増加する稲力の量")]
        public float ricePowerIncreaseAmount = 1f;  // 一回の収穫で増加する稲力の量

        [Header("一回の雷撃で減少する稲力の量")]
        public float ricePowerDecreaseAmount = 1f;  // 一回の雷撃で減少する稲力の量

        [Header("移動速度")]
        public float moveSpeed = 5f;                // プレイヤーの移動速度

        [Header("雷撃のクールダウン時間")]
        public float thunderCooldown = 0.3f;        // 雷撃のクールダウン時間

    }
}