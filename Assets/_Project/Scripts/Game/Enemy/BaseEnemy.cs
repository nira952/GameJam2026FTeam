using UnityEngine;
using UnityEngine.Pool;

// 抽象クラスとして定義（このクラス単体ではコンポーネントとしてアタッチできない）
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("基本ステータス")]
    [SerializeField] protected float moveSpeed = 3f; // 移動速度

    [Min(1)]
    [Tooltip("敵の最大体力")]
    [SerializeField] private int maxHp = 1; // 最大HP

    protected Transform targetTransform;
    private IObjectPool<BaseEnemy> originPool;

    private int currentHp; // 現在のHP
    private int thunderLayerIndex; // キャッシュ用レイヤー番号

    // プールと目標のセット（Spawnerから呼ばれる）
    public void SetPool(IObjectPool<BaseEnemy> pool) => originPool = pool;
    public void SetTarget(Transform target) => targetTransform = target;

    protected virtual void Awake()
    {
        // レイヤーの番号をあらかじめ取得（文字列比較より軽量化するため）
        thunderLayerIndex = LayerMask.NameToLayer("Thunder");
        if (thunderLayerIndex == -1)
        {
            Debug.LogError($"{gameObject.name}: 「Thunder」という名前のレイヤーがUnityプロジェクトに存在しません！");
        }
    }

    // スポーン時に毎回リセットしたい処理（HP満タン化など）
    public virtual void OnSpawn()
    {
        // プールから再利用された際に、HPを満タンにリセットする
        currentHp = maxHp;
    }

    protected virtual void Update()
    {
        if (targetTransform == null) return;

        // ゲームがプレイ中でなければ処理を行わない
        if (GameManager.Instance.CurrentGameState != GameState.Playing) return;

        // 共通の移動処理
        Move();
    }

    // 移動処理（子クラスで上書きできるように virtual にしておく）
    protected virtual void Move()
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 進行方向を向く
        RotateTowardsTarget(direction);
    }

    // 向き変更の共通処理
    protected void RotateTowardsTarget(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    // ダメージを与える用の汎用パブリック関数
    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            ReturnToPool();
        }
    }

    // Thunderレイヤーを持つオブジェクトの接触を検知する
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトのレイヤーが「Thunder」なら
        if (collision.gameObject.layer == thunderLayerIndex)
        {
            // HPを-1する
            TakeDamage(1);
        }
    }

    // プールへ戻る共通処理
    public void ReturnToPool()
    {
        if (originPool != null)
        {
            originPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}