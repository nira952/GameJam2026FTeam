using Rina_Script;
using Simizu;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class MegaTaikoScript : MonoBehaviour, IInteractable
{
    // --- シングルトンパターンの実装 ---
    #region Singleton
    public static MegaTaikoScript Instance { get; private set; }

    private void Awake()
    {
        // すでにインスタンスが存在しているかチェック
        if (Instance != null && Instance != this)
        {
            // 重複している場合は自身を破棄
            Destroy(gameObject);
            return;
        }

        // インスタンスを自身に設定
        Instance = this;
    }
    #endregion

    [SerializeField] private MegaTaikoUIScript megaTaikoUI; // MegaTaikoUIScriptの参照

    [SerializeField] private float maxRicePower = 100f;   // 最大稲パワー
    [SerializeField] private float currentRicePower = 0f; // 現在の稲パワー

    [SerializeField] private float amount = 5f; // 叩いたときに加算される稲パワーの量

    private bool isMegaTaikoModeActive = false; // メガ太鼓モードの状態

    public bool IsMegaTaikoModeActive => isMegaTaikoModeActive;


    private void Start()
    {
        megaTaikoUI.SettingRiceSlider(maxRicePower);
    }


    public void AddRicePower()
    {
        currentRicePower += amount;
        currentRicePower = Mathf.Clamp(currentRicePower, 0f, maxRicePower);

        megaTaikoUI.UpdateRiceSlider(currentRicePower);

        if (currentRicePower >= maxRicePower)
        {
            Debug.Log("稲パワーが最大値に達しました！");
        }

    }

    public string GetInteractPrompt()
    {
        return "叩く";
    }

    public PlayerState Interact()
    {
        // すでに有効なら処理しない
        if (isMegaTaikoModeActive) { return PlayerState.MegaThunder; }

        // ストックされた稲パワーが一定値以上なら、メガ太鼓モードを有効にする
        if (currentRicePower < 100f) 
        { 
            megaTaikoUI.PlayErrorAnimation();
            return PlayerState.Move; 
        }

        isMegaTaikoModeActive = true; // メガ太鼓モードを有効にする

        Debug.Log("メガ太鼓モードON！");

        //TODO : ここにカメラ移動の処理を追加する

        CameraManager.Instance.ChangeCamera(CameraPos.Center);

        return PlayerState.MegaThunder; // メガ雷撃モードに遷移する
    }

    private void Update()
    {
        if (isMegaTaikoModeActive)
        {
            // 継続的に稲パワーを消費する処理
            currentRicePower -= Time.deltaTime * 10f; // 例: 1秒あたり10の稲パワーを消費
            megaTaikoUI.UpdateRiceSlider(currentRicePower);

            if (currentRicePower <= 0f)
            {
                isMegaTaikoModeActive = false; // メガ太鼓モードを無効にする
                CameraManager.Instance.ResetCamera();
                Debug.Log("メガ太鼓モードOFF！");
            }
        }
    }
}
