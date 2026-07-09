using System;
using UnityEngine;
namespace Rina_Script
{

    // プレイヤーの収穫処理を担当するスクリプト
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHarvest : MonoBehaviour
    {
        private PlayerRoot playerRoot;

        public event Action OnHarvestAction; //収穫されたことを外部に知らせるイベント


        private bool canPlayAudio = true; // 収穫音を再生できるかどうかのフラグ
        private float audioInterval = 0.1f; // 収穫音の再生間隔
        private float lastAudioTime = 0f; // 最後に音を再生した時間


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
                if (rice.Harvest())
                {
                    if (canPlayAudio)
                    {
                        AudioManager.Instance.Play(SeName.ButtonClick); // 収穫音を鳴らす
                        canPlayAudio = false; // 収穫音を再生できない状態にする
                        lastAudioTime = Time.time; // 最後に音を再生した時間を更新
                    }


                    playerRoot.AddRicePower(); // プレイヤー稲力を増加させる関数を呼ぶ
                    OnHarvestAction?.Invoke(); // 収穫イベントを発火
                }
            }
        }

        private void Update()
        {
            // 収穫音の再生間隔を制御する
            if (Time.time - lastAudioTime >= audioInterval)
            {
                lastAudioTime = Time.time;
                canPlayAudio = true; // 収穫音を再生できる状態に戻す
            }
        }

    }
}