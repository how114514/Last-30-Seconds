using UnityEngine;

/// <summary>
/// Deals damage on contact. Two modes:
///   UseTrigger = true  → OnTriggerEnter2D  (player attack hitbox, enabled by animation)
///   UseTrigger = false → OnCollisionEnter2D (enemy body collider, knocks self back on hit)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    public enum DamageSource { Fixed, RuntimeAttack, RuntimeSlashWave }

    [Header("Damage")]
    private int m_Damage;
    [SerializeField] private DamageSource m_DamageSource;

    public void SetDamage(int damage) => m_Damage = damage;
    public void SetDamageSource(DamageSource source) => m_DamageSource = source;

    private int EffectiveDamage
    {
        get
        {
            if (RuntimeData.Instance == null) return m_Damage;

            return m_DamageSource switch
            {
                DamageSource.RuntimeAttack    => RuntimeData.Instance.attackDamage,
                DamageSource.RuntimeSlashWave => RuntimeData.Instance.slashWaveDamage,
                _                            => m_Damage
            };
        }
    }

    [Header("Target")]
    [SerializeField] private LayerMask m_TargetLayer;

    [Header("Mode")]
    [SerializeField] private bool m_UseTrigger = true;

    [Header("Knockback (collision mode only)")]
    [SerializeField] private float m_KnockbackForce = 8f;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = m_UseTrigger;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_UseTrigger) return;
        DealDamage(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_UseTrigger) return;

        if (DealDamage(collision.gameObject))
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            var movement = GetComponent<EnemyMovement>();
            if (movement != null)
                movement.ApplyKnockback(dir, m_KnockbackForce);
        }
    }

    private bool DealDamage(GameObject target)
    {
        if ((m_TargetLayer & (1 << target.layer)) == 0) return false;

        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return false;

        damageable.TakeDamage(EffectiveDamage);
        return true;
    }
}
