using Rina_Script;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private PlayerRoot playerRoot;
    private PlayerInputController playerInputController;
    private PlayerHarvest playerHarvest;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(PlayerRoot playerRoot, PlayerInputController playerInputController, PlayerHarvest playerHarvest)
    {
        this.playerInputController = playerInputController;
        this.playerHarvest = playerHarvest;
        this.playerRoot = playerRoot;

        SettingActions();

    }

    private void SettingActions()
    {

        if (playerInputController == null)
        {
            Debug.LogError("PlayerInputController is not assigned.");
            return;
        }


        playerHarvest.OnHarvestAction += OnHarvest;

        // プレイヤーの移動入力イベントに登録
        playerInputController.OnMoveAction += OnMove;
        playerInputController.OnThunderPressed += OnThunder;
    }

    public void SettingOrderLayer(int layer)
    {
        spriteRenderer.sortingOrder = layer;
    }



    private void OnMove(Vector2 movementInput)
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing ||
            playerRoot.CurrentState != PlayerState.Move)
        {
            // ゲームがプレイ中でない場合、アニメーションを再生しない
            return;
        }

        if (movementInput == Vector2.zero)
        {
            // 移動入力がない場合、待機アニメーションを再生
            animator.Play("Player_Idle");
            return;
        }

        // 移動アニメーションの再生
        animator.Play("Player_Move");

        // 移動方向に応じてオブジェクトのscaleを変更する
        if (movementInput.x > 0)
        {
            // 右に移動している場合、オブジェクトを右向きにする
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movementInput.x < 0)
        {
            // 左に移動している場合、オブジェクトを左向きにする
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnThunder(Vector3 vector, bool isPressed)
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing)
        {
            // ゲームがプレイ中でない場合、アニメーションを再生しない
            return;
        }


        if (isPressed) 
        {
            // 攻撃アニメーションの再生
            animator.Play("Player_Attack");
        }
    }

    private void OnHarvest()
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing)
        {
            // ゲームがプレイ中でない場合、アニメーションを再生しない
            return;
        }


        // 収穫アニメーションの再生
        animator.Play("Player_Happy");
    }

    private void OnDestroy()
    {
        // プレイヤーの移動入力イベントから登録解除
        if (playerInputController != null)
        {
            playerInputController.OnMoveAction -= OnMove;
            playerInputController.OnThunderPressed -= OnThunder;
            playerHarvest.OnHarvestAction -= OnHarvest;

        }

    }

}