using DG.Tweening;
using Rina_Script;
using UnityEngine;


public class PlayerMegaThunder : MonoBehaviour
{


    // --- 参照 ---
    private PlayerInputController inputController;
    private PlayerRoot playerRoot;

    private int thunderCount = 0; // 雷撃の回数をカウントする変数

    [SerializeField] GameObject thunder;
    [SerializeField] GameObject effect;

    [SerializeField] private int maxThunderCount = 0;



    public void Initialize(PlayerRoot playerRoot, PlayerInputController inputController)
    {
        this.playerRoot = playerRoot;
        this.inputController = inputController;

        SettingActions();
    }

    private void SettingActions()
    {
        if (inputController == null)
        {
            Debug.LogError("PlayerInteractor: InputController is null.");
            return;
        }

        inputController.OnThunderPressed += MegaThunder;
    }

    private void MegaThunder(Vector3 direction, bool isPressed)
    {
        // ゲームの状態がPlayingでない場合は雷撃できない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }

        // プレイヤーの状態がThunderの場合にのみ雷撃可能
        if (playerRoot.CurrentState != PlayerState.MegaThunder) { return; }

        // MegaTaikoモードが有効でない場合は雷撃できない
        if (MegaTaikoScript.Instance.IsMegaTaikoModeActive == false) { return; }

        if (!isPressed) { return; }

        Vector3 enemyPos = SpawnManager.Instance.GetRandomActiveEnemiePos(); 

        if (enemyPos == Vector3.zero)
        {
            Debug.Log("アクティブな敵が存在しません。");
            return;
        }

        // 雷撃処理を実行
        ExecuteThunderAttack(enemyPos);
    }

    // 雷撃の実行処理
    private void ExecuteThunderAttack(Vector3 direction)
    {
        Debug.Log($"雷撃を実行しました。方向: {direction}");

        AudioManager.Instance.Play(SeName.Thunder);
        AudioManager.Instance.Play(SeName.Don);


        // 雷撃の回数をカウント
        thunderCount++;

        if (thunderCount >= maxThunderCount)
        {
            thunderCount = 0;
            // 3回雷撃したら,一度大きな雷撃を落とす
            GameObject go = Instantiate(thunder, direction, Quaternion.identity);

            GameObject ef = Instantiate(effect, new Vector3(0,0,0), Quaternion.identity);
            ef.transform.DOScale(new Vector3(100,100,0),0.3f);

            Destroy(ef, 0.5f);
            Destroy(go, 1.0f);
            Debug.Log("大雷撃を落としました！");
        }
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄されるときは、メモリリーク防止のために必ずイベント解除（購読解除）を行う
        if (playerRoot != null && inputController != null)
        {
            inputController.OnThunderPressed -= MegaThunder;
        }
    }
}


