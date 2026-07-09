using UnityEngine;
namespace Rina_Script
{

    public enum PlayerState
    {
        Move,       // 移動中
        Thunder,    // 雷撃モード中
        MegaThunder // メガ雷撃モード中
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
        [SerializeField] private PlayerMegaThunder playerMegaThunder;         // メガ雷撃処理を担当するスクリプト  
        [SerializeField] private PlayerAnimator playerAnimator;               // プレイヤーのアニメーションを担当するスクリプト

        // --- プレイヤーのパラメータ---

        [Header("プレイヤーのパラメータ")]
        [SerializeField] private PlayerSettingData playerData;                // プレイヤーの設定データ

        [SerializeField] private PlayerState currentState = PlayerState.Move; // 現在のプレイヤーの状態

        public PlayerState CurrentState => currentState; // 現在のステートを外部から参照するためのプロパティ

        [SerializeField] private float currentRicePower = 0; // 現在の稲力

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

            playerInputController.Initialize(this);
            playerHarvest.Initialize(this);
            playerUIManager.Initialize(playerData.maxRicePower, playerData.currentRicePower, playerInputController, this);
            playerInteractor.Initialize(this, playerInputController, playerUIManager);
            playerMove.Initialize(this, playerInputController, playerData.moveSpeed);
            playerThunder.Initialize(this, playerInputController, playerData.thunderCooldown);
            playerMegaThunder.Initialize(this, playerInputController);
            playerAnimator.Initialize(this, playerInputController, playerHarvest);

            // 現在の稲力を設定データから取得
            currentRicePower = playerData.currentRicePower;

            // ゲーム開始時はカーソルを非表示＆画面中央にロック
            GameManager.Instance.SetCursorState(false);
        }
        


        public void ChangeState(PlayerState state)
        {
            currentState = state;

            switch (currentState)
            {
                case PlayerState.Move:
                    GameManager.Instance.SetCursorState(false);
                    playerAnimator.SettingOrderLayer(4);
                    playerUIManager.TransparentCanvasGroup(1f);

                    break;
                case PlayerState.Thunder:
                    GameManager.Instance.SetCursorState(true);
                    playerAnimator.SettingOrderLayer(7);
                    playerUIManager.TransparentCanvasGroup(0.5f);

                    break;
                case PlayerState.MegaThunder:

                    break;
            }


        }

        public bool CanThunder()
        {
            if (isDebugMode)
            {
                // 稲力が0以上なら使用可能
                return currentRicePower >= 0;
            }

            // 稲力が必要数以上の場合は雷撃可能
            return currentRicePower > playerData.ricePowerDecreaseAmount;
        }

        /// <summary>
        /// 稲力を追加する処理
        /// </summary>
        public void AddRicePower()
        {
            // 稲力を増加させる
            currentRicePower += playerData.ricePowerIncreaseAmount;

            // 稲力が最大値を超えないように制限する
            currentRicePower = Mathf.Clamp(currentRicePower, 0, playerData.maxRicePower);

            // UIのスライダーを更新する
            playerUIManager.UpdateRicePowerSlider(currentRicePower);

            // SEを鳴らす
            // AudioManager.Instance.Play(SeName.Attack);
        }


        /// <summary>
        /// 稲力を減少させる処理
        /// </summary>
        public void RemoveRicePower()
        {

            // 稲力を減少させる
            currentRicePower -= playerData.ricePowerDecreaseAmount;

            // 稲力が最小値を超えないように制限する
            currentRicePower = Mathf.Clamp(currentRicePower, 0, playerData.maxRicePower);

            // UIのスライダーを更新する
            playerUIManager.UpdateRicePowerSlider(currentRicePower);


        }

    }
}