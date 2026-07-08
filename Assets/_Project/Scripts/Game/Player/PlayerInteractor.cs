using Simizu;
using UnityEngine;
namespace Rina_Script
{


    // プレイヤーのインタラクト処理を担当するスクリプト
    public class PlayerInteractor : MonoBehaviour
    {
        private PlayerRoot playerRoot;
        private PlayerInputController inputController;

        private IInteractable currentInteractable;

        public void Initialize(PlayerRoot playerRoot, PlayerInputController inputController)
        {
            this.playerRoot = playerRoot;
            this.inputController = inputController;

            if (inputController != null)
            {
                inputController.OnInteractPressed += PerformInteract;
            }
        }


        private void PerformInteract()
        {
            // 目の前にインタラクト対象がないなら何もしない
            if (currentInteractable == null) return;


            // 現在のステートがMove状態の場合のみ、ステートを切り替える
            if (playerRoot.CurrentState == PlayerState.Move)
            {
                // ステートを取得
                PlayerState state = currentInteractable.Interact();

                if (state == PlayerState.Move) { CameraManager.Instance.ResetCamera(); }

                playerRoot.ChangeState(state);
            }
            else
            {
                CameraManager.Instance.ResetCamera();
                playerRoot.ChangeState(PlayerState.Move);
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 接触したオブジェクトが「インターフェース」を持っているか確認
            if (collision.TryGetComponent<IInteractable>(out var interactable))
            {
                currentInteractable = interactable;
                Debug.Log($"近くにターゲットを検知: {currentInteractable.GetInteractPrompt()}");
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // 離れたら対象をクリア
            if (collision.TryGetComponent<IInteractable>(out var interactable))
            {
                if (currentInteractable == interactable)
                {
                    currentInteractable = null;
                }
            }
        }


        private void OnDestroy()
        {
            // オブジェクトが破棄されるときは、メモリリーク防止のために必ずイベント解除（購読解除）を行う
            if (playerRoot != null && inputController != null)
            {
                inputController.OnInteractPressed -= PerformInteract;
            }
        }

    }
}