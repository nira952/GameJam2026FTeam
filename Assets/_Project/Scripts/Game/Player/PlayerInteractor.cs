using Simizu;
using UnityEngine;
namespace Rina_Script
{


    // プレイヤーのインタラクト処理を担当するスクリプト
    public class PlayerInteractor : MonoBehaviour
    {
        private PlayerRoot playerRoot;
        private PlayerInputController inputController;
        private PlayerUIManager uiManager;

        private IInteractable currentInteractable;

        public void Initialize(PlayerRoot playerRoot, PlayerInputController inputController,PlayerUIManager uiManager)
        {
            this.playerRoot = playerRoot;
            this.inputController = inputController;
            this.uiManager = uiManager;

            if (inputController == null)
            {
                Debug.LogError("PlayerInteractor: InputController is null.");
                return;
            }

                inputController.OnInteractPressed += PerformInteract;
            
        }


        private void PerformInteract(bool isPressed)
        {
            // 目の前にインタラクト対象がないなら何もしない
            if (currentInteractable == null) return;

            if (!isPressed) return;

            // ステートを取得
            PlayerState state = currentInteractable.Interact(transform.parent.transform);

            if (state == PlayerState.Move) { CameraManager.Instance.ResetCamera(); }

            playerRoot.ChangeState(state);


        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 接触したオブジェクトが「インターフェース」を持っているか確認
            if (collision.TryGetComponent<IInteractable>(out var interactable))
            {
                currentInteractable = interactable;
                Debug.Log($"近くにターゲットを検知: {currentInteractable.GetInteractPrompt()}");

                if (playerRoot.CurrentState == PlayerState.Move)
                {
                    // UIにインタラクト可能なオブジェクトがあることを通知
                    uiManager.UpdateInteractText(currentInteractable.GetInteractPrompt(), false);
                }
                else if (playerRoot.CurrentState == PlayerState.Thunder || playerRoot.CurrentState == PlayerState.MegaThunder)
                {
                    // UIにインタラクト可能なオブジェクトがあることを通知
                    uiManager.UpdateInteractText(currentInteractable.GetInteractPrompt(), true);

                }


            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // 離れたら対象をクリア
            if (collision.TryGetComponent<IInteractable>(out var interactable))
            {
                if (currentInteractable == interactable)
                {
                    uiManager.HideInteractText();
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