using UnityEngine;

/// <summary>
/// Deals damage on contact. Two modes:
///   UseTrigger = true  → OnTriggerEnter2D  (player attack hitbox, enabled by animation)
///   UseTrigger = false → OnCollisionEnter2D (enemy body collider, knocks self back on hit)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int m_Damage = 10;

    public void SetDamage(int damage) => m_Damage = damage;

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

        damageable.TakeDamage(m_Damage);
        return true;
    }
}
