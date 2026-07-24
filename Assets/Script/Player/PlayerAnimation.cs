using UnityEngine;

/// <summary>
/// Pure animation component. States call PlayIdle / PlayMove / PlayAttack.
/// Facing is handled via ApplyFacing and locked during attack.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_AttackSound;

    private Animator m_Animator;
    private bool m_FacingLocked;

    private static readonly int s_IdleHash   = Animator.StringToHash("idle");
    private static readonly int s_MoveHash   = Animator.StringToHash("move");
    private static readonly int s_AttackHash = Animator.StringToHash("attack");
    private static readonly int s_HurtHash   = Animator.StringToHash("hurt");

    public AnimatorStateInfo CurrentAnimState => m_Animator.GetCurrentAnimatorStateInfo(0);

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    // ── Animation commands ──────────────────────────────────────────

    public void PlayIdle()
    {
        m_Animator.speed = 1f;
        m_Animator.Play(s_IdleHash, 0, 0f);
    }

    public void PlayMove()
    {
        m_Animator.speed = 1f;
        m_Animator.Play(s_MoveHash, 0, 0f);
    }

    public void PlayAttack()
    {
        m_Animator.speed = RuntimeData.Instance != null ? RuntimeData.Instance.attackSpeed : 1f;
        m_Animator.Play(s_AttackHash, 0, 0f);

        if (m_AudioSource != null && m_AttackSound != null)
            m_AudioSource.PlayOneShot(m_AttackSound);
    }

    public void PlayHurt()
    {
        m_Animator.speed = 1f;
        m_Animator.Play(s_HurtHash, 0, 0f);
    }

    // ── Facing ──────────────────────────────────────────────────────

    public void ApplyFacing(float moveX)
    {
        if (m_FacingLocked) return;

        Vector3 scale = transform.localScale;

        if (moveX > 0.01f)
            scale.x = Mathf.Abs(scale.x);   // face right
        else if (moveX < -0.01f)
            scale.x = -Mathf.Abs(scale.x);  // face left

        transform.localScale = scale;
    }

    public void LockFacing()   => m_FacingLocked = true;
    public void UnlockFacing() => m_FacingLocked = false;
}
