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

    [Header("Feedback")]
    [SerializeField] private float m_FlashDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_HurtSound;
    [SerializeField] private AudioClip m_DeathSound;

    private PlayerScore m_PlayerScore;
    private SpriteRenderer m_SpriteRenderer;
    private Coroutine m_FlashRoutine;

    public int MaxHealth => m_MaxHealth;
    public int CurrentHealth => m_CurrentHealth;
    public bool IsDead => m_CurrentHealth <= 0;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
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
        StartFlash();

        if (m_AudioSource != null && m_HurtSound != null)
            m_AudioSource.PlayOneShot(m_HurtSound);

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (m_PlayerScore != null)
            m_PlayerScore.AddScore(m_ScoreReward);

        // Disable collider + movement so the corpse stays briefly for the sound
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        var move = GetComponent<EnemyMovement>();
        if (move != null) move.enabled = false;

        if (m_AudioSource != null && m_DeathSound != null)
            m_AudioSource.PlayOneShot(m_DeathSound);

        Destroy(gameObject, 0.5f);
    }

    private void StartFlash()
    {
        if (m_SpriteRenderer == null) return;
        if (m_FlashRoutine != null) StopCoroutine(m_FlashRoutine);
        m_FlashRoutine = StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        m_SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(m_FlashDuration);
        m_SpriteRenderer.color = Color.white;
    }

    /// <summary>Destroy without awarding score — used at game end.</summary>
    public void DieSilently()
    {
        Destroy(gameObject);
    }
}
