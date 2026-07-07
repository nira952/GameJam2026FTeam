using UnityEngine;
namespace Rina_Script
{

    // プレイヤーの収穫処理を担当するスクリプト
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHarvest : MonoBehaviour
    {
        private PlayerRoot playerRoot;

        public void Initialize(PlayerRoot playerRoot)
        {
            this.playerRoot = playerRoot;
        }


        // トリガーコライダーに何かが触れた時にUnityが自動で呼ぶ関数
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 触れた相手がRicePlantScriptを持っているか確認
            if (collision.TryGetComponent(out RicePlantScript rice))
            {
                // 稲の収穫関数を呼ぶ
                rice.Harvest();
                playerRoot.AddRicePower(); // プレイヤー稲力を増加させる関数を呼ぶ
            }
        }
    }
}