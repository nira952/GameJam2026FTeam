using UnityEngine;
namespace Rina_Script
{

    // プレイヤーの移動処理を担当するスクリプト
    public class PlayerMove : MonoBehaviour
    {
        // --- 参照 ---
        private PlayerRoot playerRoot;                   // プレイヤーのルートスクリプトへの参照
        private PlayerInputController inputController;   // プレイヤーの入力コントローラへの参照

        // --- 変数 ---
        private float moveSpeed;                         // プレイヤーの移動速度


        // 初期化処理
        public void Initialize(PlayerRoot playerRoot, PlayerInputController inputController, float moveSpeed)
        {
            this.playerRoot = playerRoot;
            this.inputController = inputController;
            this.moveSpeed = moveSpeed;

            // 入力イベントの設定
            SettingActions();
        }

        private void SettingActions()
        {
            inputController.OnMove += Move;
        }

        private void Update()
        {
            // デバッグ用の仮の移動処理（WASDキーでの移動）
            Move(Vector2.down);
        }

        private void Move(Vector2 vector)
        {
            // ゲームの状態がPlayingでない場合は移動できない
            if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

            // プレイヤーの状態がMoveの場合にのみ移動可能
            if (playerRoot.CurrentState != PlayerState.Move) { return; }


            //// 仮の移動処理（WASDキーでの移動）

            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(moveX, moveY, 0f).normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

    }

}