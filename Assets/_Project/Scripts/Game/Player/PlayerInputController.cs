using System;
using UnityEngine;
namespace Rina_Script
{


    // プレイヤーの入力を管理するスクリプト
    public class PlayerInputController : MonoBehaviour
    {
        // インタラクトボタンが押されたことを外部に知らせるイベント
        public event Action OnInteractPressed;

        public event Action<Vector2> OnMove; // 移動入力があったことを外部に知らせるイベント

        public event Action<Vector3> OnThunderPressed; // 雷撃ボタンが押されたことを外部に知らせるイベント

        private void Update()
        {
            // InputManagerの仮システム

            //// インタラクトボタンが押されたかをチェック
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // イベントを発火させる（購読者に通知）
                OnInteractPressed?.Invoke();
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnThunderPressed?.Invoke(Input.mousePosition);
            }


        }

    }
}