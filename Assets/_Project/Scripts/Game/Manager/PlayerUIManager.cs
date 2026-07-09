using Rina_Script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// プレイヤーのUI管理を担当するスクリプト
public class PlayerUIManager : MonoBehaviour
{
    private PlayerRoot playerRoot; // プレイヤーのルートオブジェクトへの参照
    private PlayerInputController inputController; // プレイヤーの入力コントローラーへの参照

    [SerializeField] private Slider ricePowerSlider; // 米の力スライダー

    [SerializeField] private TextMeshPro interactText; // インタラクトテキスト

    [Header("移動キー0:W, 1:A, 2:S, 3:D")]
    [SerializeField] private Image[] keyImages = new Image[4]; // 移動キーの画像配列

    [SerializeField] private Sprite[] keyDefaultSprites = new Sprite[4]; // 移動キーのスプライト配列

    [SerializeField] private Sprite[] keyPressedSprites = new Sprite[4]; // 押された移動キーのスプライト配列

    [SerializeField] private GameObject spaceKeyParent;

    [SerializeField] private GameObject leftMouseKeyParent;


    [Header("インタラクトキー")]
    [SerializeField] private SpriteRenderer spaceKeyImage; // インタラクトキーの画像
    [SerializeField] private Sprite spaceKeyDefaultSprite; // インタラクトキーのデフォルトスプライト
    [SerializeField] private Sprite spaceKeyPressedSprite; // インタラクトキーの押されたスプライト

    [Header("アタックキー")]
    [SerializeField] private SpriteRenderer leftMouseKeyImage; // アタックキーの画像
    [SerializeField] private Sprite leftMouseKeyDefaultSprite; // アタックキーのデフォルトスプライト
    [SerializeField] private Sprite leftMouseKeyPressedSprite; // アタックキーの押されたスプライト

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="maxRicePower"></param>
    public void Initialize(float maxRicePower,float currentRicePower , PlayerInputController inputController, PlayerRoot root)
    {
       SettingMaxRicePower(maxRicePower,currentRicePower);
       this.inputController = inputController;
       this.playerRoot = root;

        SettingActions();
    }

    private void SettingActions()
    {
        if (inputController == null)
        {
            Debug.LogError("PlayerInputControllerがセットされていません。");
            return;
        }

        inputController.OnMoveAction += ActionMoveKey;
        inputController.OnInteractPressed += ActionSpaceKey;
        inputController.OnThunderPressed += ActionLeftMouseKey;
    }



    // スライダーの最大値を設定する関数
    private void SettingMaxRicePower(float power,float currentPower)
    {
        ricePowerSlider.maxValue = power;      // スライダーの最大値を設定
        ricePowerSlider.value = currentPower;  // スライダーの初期値を設定

    }

    /// <summary>
    /// スライダーの値を更新する関数
    /// </summary>
    /// <param name="value"></param>
    public void UpdateRicePowerSlider(float value)
    {
        ricePowerSlider.value = value;

    }

    public void UpdateInteractText(string text,bool isAttack)
    {

        // テキストを表示する
        interactText.text = text;
        
        spaceKeyParent.SetActive(true);

        if (isAttack)
        {
            leftMouseKeyParent.SetActive(true);
        }

    }

    public void HideInteractText()
    {
        spaceKeyParent.SetActive(false);
        leftMouseKeyParent.SetActive(false);
    }

    private void ActionMoveKey(Vector2 vector)
    {
        // ゲームの状態がPlayingでない場合は何もしない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }
        // プレイヤーの状態がMoveの場合にのみ処理可能
        if (playerRoot.CurrentState != PlayerState.Move) { return; }

        // 入力ベクトルの方向に応じて、対応するキーの画像を押された状態に変更
        if (vector.y > 0) // 上方向の入力
        {
            keyImages[0].sprite = keyPressedSprites[0]; // Wキーを押された状態に変更
        }
        else
        {
            keyImages[0].sprite = keyDefaultSprites[0]; // Wキーをデフォルト状態に戻す
        }

        if (vector.x < 0) // 左方向の入力
        {
            keyImages[1].sprite = keyPressedSprites[1]; // Aキーを押された状態に変更
        }
        else
        {
            keyImages[1].sprite = keyDefaultSprites[1]; // Aキーをデフォルト状態に戻す
        }

        if (vector.y < 0) // 下方向の入力
        {
            keyImages[2].sprite = keyPressedSprites[2]; // Sキーを押された状態に変更
        }
        else
        {
            keyImages[2].sprite = keyDefaultSprites[2]; // Sキーをデフォルト状態に戻す
        }

        if (vector.x > 0) // 右方向の入力
        {
            keyImages[3].sprite = keyPressedSprites[3]; // Dキーを押された状態に変更
        }
        else
        {
            keyImages[3].sprite = keyDefaultSprites[3]; // Dキーをデフォルト状態に戻す
        }




    }

    private void ActionSpaceKey(bool isPressed)
    {
        // ゲームの状態がPlayingでない場合は何もしない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }
        // プレイヤーの状態がMoveの場合にのみ処理可能
        if (playerRoot.CurrentState != PlayerState.Move) { return; }
        if (isPressed)
        {
            spaceKeyImage.sprite = spaceKeyPressedSprite; // スペースキーを押された状態に変更
        }
        else
        {
            spaceKeyImage.sprite = spaceKeyDefaultSprite; // スペースキーをデフォルト状態に戻す
        }
    }

    private void ActionLeftMouseKey(Vector3 direction, bool isPressed)
    {
        // ゲームの状態がPlayingでない場合は何もしない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) { return; }
        // プレイヤーの状態がThunderの場合にのみ処理可能
        if (playerRoot.CurrentState != PlayerState.Thunder) { return; }


        if (isPressed)
        {
            leftMouseKeyImage.sprite = leftMouseKeyPressedSprite; // 左クリックを押された状態に変更
        }
        else
        {
            leftMouseKeyImage.sprite = leftMouseKeyDefaultSprite; // 左クリックをデフォルト状態に戻す
        }
    }

}
