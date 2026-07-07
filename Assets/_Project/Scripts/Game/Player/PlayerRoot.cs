using UnityEngine;
namespace Rina_Script
{

    public enum PlayerState
    {
        Move,   // 移動中
        Thunder // 雷撃中
    }

    // プレイヤーのルートスクリプト
    public class PlayerRoot : MonoBehaviour
    {
        // --- 各コンポーネント参照 ---

        [Header("プレイヤーのコンポーネント")]
        [SerializeField] private PlayerUIManager playerUIManager;             // プレイヤーのUI管理を担当するスクリプト
        [SerializeField] private PlayerHarvest playerHarvest;                 // 収穫処理を担当するスクリプト
        [SerializeField] private PlayerInteractor playerInteractor;           // インタラクション処理を担当するスクリプト
        [SerializeField] private PlayerMove playerMove;                       // 移動処理を担当するスクリプト
        [SerializeField] private PlayerInputController playerInputController; // プレイヤーの入力処理を担当するスクリプト
        [SerializeField] private PlayerThunder playerThunder;                 // 雷撃処理を担当するスクリプト

        // --- プレイヤーのパラメータ---

        [Header("プレイヤーのパラメータ")]
        [SerializeField] private PlayerSettingData playerData;                // プレイヤーの設定データ

        [SerializeField] private PlayerState currentState = PlayerState.Move; // 現在のプレイヤーの状態

        public PlayerState CurrentState => currentState; // 現在のステートを外部から参照するためのプロパティ

        [SerializeField] private bool isDebugMode = false; // デバッグモードのフラグ

        private void Start()
        {
            if (playerData == null)
            {
                Debug.LogError($"{gameObject.name}: PlayerSettingData がインスペクターにセットされていません！");
                enabled = false;
                return;
            }

            // --- 各コンポーネントの初期化 ---

            playerUIManager.Initialize(playerData.maxRicePower, playerData.currentRicePower);
            playerHarvest.Initialize(this);
            playerInteractor.Initialize(this, playerInputController);
            playerMove.Initialize(this, playerInputController, playerData.moveSpeed);
            playerThunder.Initialize(this, playerInputController, playerData.thunderCooldown);

        }


        public void ChangeState()
        {
            // 現在のステートがMoveの場合はThunderに切り替える
            if (currentState == PlayerState.Move)
            {
                currentState = PlayerState.Thunder;
            }
            // 現在のステートがThunderの場合はMoveに切り替える
            else if (currentState == PlayerState.Thunder)
            {
                currentState = PlayerState.Move;
            }
        }

        public bool CanThunder()
        {
            if (isDebugMode)
            {
                // 稲力が0以上なら使用可能
                return playerData.currentRicePower >= 0;
            }

            // 稲力が必要数以上の場合は雷撃可能
            return playerData.currentRicePower > playerData.ricePowerDecreaseAmount;
        }

        /// <summary>
        /// 稲力を追加する処理
        /// </summary>
        public void AddRicePower()
        {
            // 稲力を増加させる
            playerData.currentRicePower += playerData.ricePowerIncreaseAmount;

            // 稲力が最大値を超えないように制限する
            playerData.currentRicePower = Mathf.Clamp(playerData.currentRicePower, 0, playerData.maxRicePower);

            // UIのスライダーを更新する
            playerUIManager.UpdateRicePowerSlider(playerData.currentRicePower);

            // SEを鳴らす
            // AudioManager.Instance.Play(SeName.Attack);
        }


        /// <summary>
        /// 稲力を減少させる処理
        /// </summary>
        public void RemoveRicePower()
        {

            // 稲力を減少させる
            playerData.currentRicePower -= playerData.ricePowerDecreaseAmount;

            // 稲力が最小値を超えないように制限する
            playerData.currentRicePower = Mathf.Clamp(playerData.currentRicePower, 0, playerData.maxRicePower);

            // UIのスライダーを更新する
            playerUIManager.UpdateRicePowerSlider(playerData.currentRicePower);


        }

    }
}