using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Rina_Script
{


    // プレイヤーの入力を管理するスクリプト
    public class PlayerInputController : MonoBehaviour
    {
        // インタラクトボタンが押されたことを外部に知らせるイベント
        public event Action OnInteractPressed;

        public event Action<Vector2> OnMoveAction; // 移動入力があったことを外部に知らせるイベント

        public event Action<Vector3> OnThunderPressed; // 雷撃ボタンが押されたことを外部に知らせるイベント

        private Player_Action InputActions; //

        private void Awake()
        {
            InputActions = new Player_Action();

            InputActions.Player.Move.performed += OnMove;
            InputActions.Player.thunder.performed += OnThunder;
            InputActions.Player.intaract.performed += OnInteract;
        }
        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }


        private void OnMove(InputAction.CallbackContext contect)
        {
            Debug.Log("移動");
        }
        private void OnThunder(InputAction.CallbackContext contect)
        {
            Debug.Log("雷");
        }
        private void OnInteract(InputAction.CallbackContext contect)
        {
            Debug.Log("触れる");
        }
    }
}