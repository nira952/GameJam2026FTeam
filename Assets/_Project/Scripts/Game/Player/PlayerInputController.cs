using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
namespace Rina_Script
{


    // プレイヤーの入力を管理するスクリプト
    public class PlayerInputController : MonoBehaviour
    {
        PlayerRoot playerRoot;

        // インタラクトボタンが押されたことを外部に知らせるイベント
        public event Action<bool> OnInteractPressed;

        public event Action<Vector2> OnMoveAction; // 移動入力があったことを外部に知らせるイベント

        public event Action<Vector3,bool> OnThunderPressed; // 雷撃ボタンが押されたことを外部に知らせるイベント

        private Player_Action InputActions;


        private bool canInteract = true;
        private float interactCooldownTimer = 0;
        private float interactCooldownTime = 0.3f;

        public void Initialize(PlayerRoot playerRoot)
        {
            this.playerRoot = playerRoot;
        }

        private void Awake()
        {
            InputActions = new Player_Action();

            //InputActions.Player.Move.started += OnMove;
            //InputActions.Player.Move.canceled += OnMove;

            //InputActions.Player.thunder.performed += OnThunder;
            //InputActions.Player.intaract.performed += OnInteract;
        }
        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            // inputVectorはメソッド外のメンバ変数（またはプロパティ）として
            // 保持されている前提の書き方に直しています
            Vector2 inputVector = Vector2.zero;
            inputVector = context.ReadValue<Vector2>();


            OnMoveAction?.Invoke(inputVector);

            if(playerRoot.CurrentState == PlayerState.Thunder || playerRoot.CurrentState == PlayerState.MegaThunder)
            {
                OnInteractPressed?.Invoke(true);
            }

        }

        public void OnThunder(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                // 雷を落とす位置（ポイントで取得する）
                var pointer = Pointer.current;
                if (pointer == null)
                    return;

                Vector3 position = pointer.position.ReadValue();

                OnThunderPressed?.Invoke(position,true);

            }
            else if (context.canceled)
            {
                Vector3 vector = Vector3.zero;

                OnThunderPressed?.Invoke(vector,false);
            }


        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!canInteract) { return; }
            canInteract = false;
            interactCooldownTimer = interactCooldownTime;

            if (context.started)
            {
                OnInteractPressed?.Invoke(true);

            }
            else if (context.canceled)
            {
                OnInteractPressed?.Invoke(false);
            }

        }

        private void Update()
        {
            // クールダウンタイマーの更新

            if (!canInteract)
            {
                if (interactCooldownTimer <= 0)
                {
                    canInteract = true;
                }
            }

            if (interactCooldownTimer > 0)
            {
                interactCooldownTimer -= Time.deltaTime;
            }
        }

    }
}