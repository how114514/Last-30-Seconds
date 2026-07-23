using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pure movement component. Reads input directly from the InputAction
/// (lazy, no cached Update) so state-machine queries always get the
/// current frame's value regardless of script execution order.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_MoveSpeed = 5f;

    private Rigidbody2D m_Rigidbody;
    private InputAction m_MoveAction;
    private bool m_Blocked;

    /// <summary>Horizontal-only input (A/D keys).</summary>
    public Vector2 MoveInput
    {
        get
        {
            var raw = m_MoveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            return new Vector2(raw.x, 0f);
        }
    }

    /// <summary>True while A or D is held.</summary>
    public bool IsMoving => Mathf.Abs(MoveInput.x) > 0.01f;

    public Vector2 Velocity => m_Rigidbody ? m_Rigidbody.linearVelocity : Vector2.zero;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>Called by Player in Start.</summary>
    public void Initialize(InputSystem_Actions inputActions)
    {
        m_MoveAction = inputActions.Player.Move;
    }

    private void FixedUpdate()
    {
        if (m_Blocked)
        {
            m_Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        m_Rigidbody.linearVelocity = MoveInput * m_MoveSpeed;
    }

    public void BlockMovement()   => m_Blocked = true;
    public void UnblockMovement() => m_Blocked = false;
}
