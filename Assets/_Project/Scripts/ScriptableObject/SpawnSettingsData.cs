using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnSettings", menuName = "ScriptableObjects/SpawnSettings", order = 1)]
public class SpawnSettingsData : ScriptableObject
{
    [Header("--- 各敵のスポーン間隔設定（秒） ---")]
    [Tooltip("DefaultEnemyの出現間隔")]
    public float defaultEnemyInterval = 5f;

    [Tooltip("ZigZagEnemyの出現間隔")]
    public float zigZagEnemyInterval = 20f;

    [Tooltip("BossEnemyの出現間隔")]
    public float bossEnemyInterval = 60f;

    [Header("--- スポーン速度（難易度の上昇時間） ---")]
    [Min(0.01f)]
    [Tooltip("【初期の難易度】ベース間隔にかける倍率。基本は 1.0 に設定してください")]
    public float initialSpawnInterval = 1.0f;

    [Min(0.01f)]
    [Tooltip("【最大の難易度】最高速に達したときの倍率。例えば 0.5 なら指定の半分の時間（2倍の速度）で出現します")]
    public float minSpawnInterval = 0.5f;

    [Min(1f)]
    [Tooltip("【難易度上昇の猶予時間】ゲーム開始から最高速（minSpawnInterval）に達するまでに何秒かけるか")]
    public float timeToReachMaxSpeed = 300f;

    [Header("--- 配置・出現エリアの設定 ---")]
    [Min(0f)]
    [Tooltip("中心（核）から敵が湧くラインまでの直線距離。正方形の拠点サイズに合わせて調整してください")]
    public float spawnDistance = 10f;

    [Min(0f)]
    [Tooltip("出現位置の横方向へのバラつき幅。この幅の範囲内でランダムな位置にスポーンします")]
    public float positionVariance = 3f;

    [Header("--- 波（特定の方向からのラッシュ）の設定 ---")]
    [Min(0.1f)]
    [Tooltip("同じ方角から攻め続ける「ラッシュの最短時間（秒）」")]
    public float minWaveDuration = 5f;

    [Min(0.1f)]
    [Tooltip("同じ方角から攻め続ける「ラッシュの最長時間（秒）」")]
    public float maxWaveDuration = 15f;

    [Range(0f, 1f)]
    [Tooltip("現在選ばれているラッシュの方向から敵が湧く確率。1.0にすると100%その方向からしか来なくなります（0.25で完全ランダム）")]
    public float favoriteDirectionChance = 0.7f;
}