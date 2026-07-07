using UnityEngine;
using UnityEngine.UIElements;

namespace Rina_Script
{
    


    // プレイヤーの雷撃処理を担当するスクリプト
    public class PlayerThunder : MonoBehaviour
    {
        // --- 参照 ---
        private PlayerInputController inputController;
        private PlayerRoot playerRoot;

        private float thunderCooldown; // 雷撃のクールダウン時間（秒）
        private float lastThunderTime;

        private bool canThunder = true; // 雷撃が使用可能かどうかのフラグ

        [SerializeField] private GameObject thunder;

        public void Initialize(PlayerRoot playerRoot, PlayerInputController inputController, float thunderCooldown)
        {
            this.playerRoot = playerRoot;
            this.inputController = inputController;
            this.thunderCooldown = thunderCooldown;

            SettingActions();
        }

        private void SettingActions()
        {
            inputController.OnThunderPressed += Thunder;
        }

        private void Thunder(Vector3 direction)
        {
            // ゲームの状態がPlayingでない場合は雷撃できない
            if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

            // プレイヤーの状態がThunderの場合にのみ雷撃可能
            if (playerRoot.CurrentState != PlayerState.Thunder) { return; }

            // 雷撃のクールダウン中は雷撃できない
            if (!canThunder) { return; }

            // プレイヤーが稲パワーを持っているか確認
            if (!playerRoot.CanThunder())
            {
                // 稲パワーが不足している場合の処理
                Debug.Log("稲パワーが不足しています。雷撃できません。");
                return;
            }

            // 雷撃処理を実行する前に、プレイヤーの稲パワーを消費する
            playerRoot.RemoveRicePower();

            // クールダウンを開始する
            canThunder = false;
            lastThunderTime = Time.time;


            // 雷撃処理を実行
            ExecuteThunderAttack(direction);


        }

        // 雷撃の実行処理
        private void ExecuteThunderAttack(Vector3 direction)
        {
            Debug.Log($"雷撃を実行しました。方向: {direction}");

            direction.z = -Camera.main.transform.position.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(direction);
            Debug.Log($"World position{worldPos}");

            GameObject go =  Instantiate(thunder, worldPos, Quaternion.identity);

            Destroy(go,2f);

        }
        private void Update()
        {
            ThunderCoolDown();
        }

        private void ThunderCoolDown()
        {
            // クールダウン処理
            if (Time.time >= lastThunderTime + thunderCooldown)
            {
                // クールダウンが終了した場合の処理
                canThunder = true;
            }
        }


        private void OnDestroy()
        {
            // オブジェクトが破棄されるときは、メモリリーク防止のために必ずイベント解除（購読解除）を行う
            if (playerRoot != null && inputController != null)
            {
                inputController.OnThunderPressed -= Thunder;
            }
        }
    }

}