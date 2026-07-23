using UnityEngine;

/// <summary>
/// Moves the enemy toward the player (found by "Player" tag in Start).
/// Flips the entire transform to face the movement direction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_MoveSpeed = 3f;

    private Transform m_PlayerTransform;
    private Rigidbody2D m_Rigidbody;

    public void SetMoveSpeed(float speed) => m_MoveSpeed = speed;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

        if (Player.Instance != null)
            m_PlayerTransform = Player.Instance.transform;
    }

    private void FixedUpdate()
    {
        if (m_PlayerTransform == null) return;

        // Horizontal direction only
        float toPlayerX = m_PlayerTransform.position.x - transform.position.x;
        float directionX = Mathf.Sign(toPlayerX);

        // Move on X axis only
        var vel = m_Rigidbody.linearVelocity;
        vel.x = directionX * m_MoveSpeed;
        m_Rigidbody.linearVelocity = vel;

        // Enemy sprite faces left by default, so flip when moving right
        Vector3 scale = transform.localScale;
        if (directionX > 0.01f)
            scale.x = -Mathf.Abs(scale.x);  // moving right → flip to face right
        else if (directionX < -0.01f)
            scale.x = Mathf.Abs(scale.x);   // moving left → default facing
    }
}
