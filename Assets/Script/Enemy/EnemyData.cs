using UnityEngine;

/// <summary>
/// ScriptableObject holding all enemy configuration data.
/// Create via: right-click in Project → Last 30 Seconds → Enemy Data.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Last 30 Seconds/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;

    [Header("Appearance")]
    public Sprite sprite;

    [Header("Stats")]
    public int hp = 100;
    public int damage = 10;
    public float moveSpeed = 3f;

    [Header("Rewards")]
    public int scoreReward = 10;

    [Header("Spawning")]
    public float spawnWeight = 10f;
    public bool isBoss;
}
