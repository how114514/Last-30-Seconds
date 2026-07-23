using UnityEngine;

/// <summary>
/// Stores enemy health. Receives damage from DamageDealer.
/// Destroys the GameObject when health reaches zero.
/// </summary>
public class EnemyStats : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int m_MaxHealth = 100;
    [SerializeField] private int m_CurrentHealth;

    [Header("Reward")]
    [SerializeField] private int m_ScoreReward = 10;

    private PlayerScore m_PlayerScore;

    public int MaxHealth => m_MaxHealth;
    public int CurrentHealth => m_CurrentHealth;
    public bool IsDead => m_CurrentHealth <= 0;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    private void Start()
    {
        if (Player.Instance != null)
            m_PlayerScore = Player.Instance.GetComponent<PlayerScore>();
    }

    /// <summary>Apply EnemyData to this enemy's components after spawn.</summary>
    public void ApplyData(EnemyData data)
    {
        m_MaxHealth     = data.hp;
        m_CurrentHealth = data.hp;
        m_ScoreReward   = data.scoreReward;
        gameObject.name = data.enemyName;

        var movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.SetMoveSpeed(data.moveSpeed);

        var dealer = GetComponent<DamageDealer>();
        if (dealer != null) dealer.SetDamage(data.damage);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = data.sprite;
    }

    /// <summary>
    /// Apply damage. Triggers death when health drops to zero or below.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - amount);

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (m_PlayerScore != null)
            m_PlayerScore.AddScore(m_ScoreReward);

        // TODO: play death animation / effects here
        Destroy(gameObject);
    }
}
