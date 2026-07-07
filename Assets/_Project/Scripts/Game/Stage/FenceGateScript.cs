using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FenceGateScript : MonoBehaviour
{
    private Collider2D gateCollider;

    private void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
        gateCollider.isTrigger = true; // トリガーとして設定
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // "Enemy" レイヤーの番号を取得
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        // 接触した相手のレイヤー番号と一致するかチェック
        if (collision.gameObject.layer == enemyLayer)
        {
            // 敵が触れたら、ゲームオーバーにする
            GameManager.Instance.GameOver();
        }
    }
}