using UnityEngine;

/// <summary>
/// Moves the enemy toward the player. Briefly stunned on knockback
/// so the impulse isn't immediately overwritten.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_MoveSpeed = 3f;

    private Transform m_PlayerTransform;
    private Rigidbody2D m_Rigidbody;
    private float m_StunTimer;

    public void SetMoveSpeed(float speed) => m_MoveSpeed = speed;

    /// <summary>Apply knockback away from player and stun briefly.</summary>
    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (m_Rigidbody == null) return;

        m_StunTimer = 0.2f;
        m_Rigidbody.linearVelocity = Vector2.zero;
        m_Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        if (Player.Instance != null)
            m_PlayerTransform = Player.Instance.transform;
    }

    private void FixedUpdate()
    {
        // Don't apply movement while stunned — let knockback play out
        if (m_StunTimer > 0f)
        {
            m_StunTimer -= Time.fixedDeltaTime;
            return;
        }

        if (m_PlayerTransform == null) return;

        float toPlayerX = m_PlayerTransform.position.x - transform.position.x;
        float directionX = Mathf.Sign(toPlayerX);

        var vel = m_Rigidbody.linearVelocity;
        vel.x = directionX * m_MoveSpeed;
        m_Rigidbody.linearVelocity = vel;

        Vector3 scale = transform.localScale;
        if (directionX > 0.01f)
            scale.x = -Mathf.Abs(scale.x);
        else if (directionX < -0.01f)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }
}
