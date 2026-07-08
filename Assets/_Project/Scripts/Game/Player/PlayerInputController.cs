using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Rina_Script
{


    // プレイヤーの入力を管理するスクリプト
    public class PlayerInputController : MonoBehaviour
    {
        PlayerRoot playerRoot;

        // インタラクトボタンが押されたことを外部に知らせるイベント
        public event Action OnInteractPressed;

        public event Action<Vector2> OnMoveAction; // 移動入力があったことを外部に知らせるイベント

        public event Action<Vector3> OnThunderPressed; // 雷撃ボタンが押されたことを外部に知らせるイベント

        public event Action OnMegaThunderPressed; // 雷撃ボタンが押されたことを外部に知らせるイベント


        private Player_Action InputActions; //

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

            return;

            // started（最初に押した時）または performed（押しながら別のキーを押して値が変わった時）
            if (context.started || context.performed)
            {
                Debug.Log("Move input updated: " + context.ReadValue<Vector2>());

                // ベクトルを取得（斜めなら（0.7, 0.7）や（1, 1）などが取得できます）

                // 【オプション】斜め移動の速度が速くなるのを防ぐ場合（正規化）
                // キーボード入力（1, 1）を滑らかな円の範囲（最大長1）に収めます
                if (inputVector.sqrMagnitude > 1f)
                {
                    inputVector.Normalize();
                }
            }
            else if (context.canceled)
            {
                // すべてのキーが離された時
                inputVector = Vector2.zero;
            }

        }

        public void OnThunder(InputAction.CallbackContext context)
        {
            if(playerRoot.CurrentState == PlayerState.Thunder)
            {
                // 雷を落とす位置（ポイントで取得する）
                var pointer = Pointer.current;
                if (pointer == null)
                    return;

                Vector3 position = pointer.position.ReadValue();
                OnThunderPressed?.Invoke(position);
            }
            else if(playerRoot.CurrentState == PlayerState.MegaThunder)
            {
                OnMegaThunderPressed?.Invoke();
            }



        }
        public void OnInteract(InputAction.CallbackContext context)
        {
           OnInteractPressed?.Invoke();
        }
    }
}