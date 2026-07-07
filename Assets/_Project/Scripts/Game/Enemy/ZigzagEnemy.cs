using UnityEngine;

public class ZigzagEnemy : BaseEnemy
{
    [Header("ジグザグ設定")]
    [SerializeField] private float frequency = 5f; // 揺れる速さ
    [SerializeField] private float magnitude = 2f; // 揺れる幅

    private float sinTime;

    public override void OnSpawn()
    {
        base.OnSpawn();
        sinTime = 0f;
    }

    // 移動処理だけを独自の動きに上書き！
    protected override void Move()
    {
        sinTime += Time.deltaTime;

        // 中央への基本ベクトル
        Vector3 baseDirection = (targetTransform.position - transform.position).normalized;

        // 直交するベクトル（横揺れ用）を計算
        Vector3 sideDirection = new Vector3(-baseDirection.y, baseDirection.x, 0f);

        // まっすぐ進む力 + サイン波による横揺れ
        Vector3 finalMovement = baseDirection * moveSpeed;
        finalMovement += sideDirection * Mathf.Sin(sinTime * frequency) * magnitude;

        transform.position += finalMovement * Time.deltaTime;

        // 進行方向を向く
        RotateTowardsTarget(baseDirection);
    }
}