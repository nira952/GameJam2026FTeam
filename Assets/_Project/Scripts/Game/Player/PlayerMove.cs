using UnityEngine;
using UnityEngine.EventSystems;
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

        private Vector2 currentDirection = Vector2.zero;

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
            inputController.OnMoveAction += ChangeDirection;
        }

        private void Update()
        {
            // ゲームの状態がPlayingでない場合は移動できない
            if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

            // プレイヤーの状態がMoveの場合にのみ移動可能
            if (playerRoot.CurrentState != PlayerState.Move) { return; }

            // 移動処理
            Move();
        }

        private void Move()
        {
            // 現在の方向に基づいてプレイヤーを移動させる
            transform.Translate(currentDirection * moveSpeed * Time.deltaTime, Space.World);

        }

        private void ChangeDirection(Vector2 vector)
        {
            // ゲームの状態がPlayingでない場合は移動できない
            if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

            // プレイヤーの状態がMoveの場合にのみ移動可能
            if (playerRoot.CurrentState != PlayerState.Move) { return; }

            Debug.Log($"ChangeDirection: {vector}");

            // プレイヤーの移動方向を更新
            currentDirection = vector.normalized;
        }

    }

}