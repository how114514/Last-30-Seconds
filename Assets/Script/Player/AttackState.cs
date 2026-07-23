using UnityEngine;

/// <summary>
/// Attack state: plays attack animation, blocks movement + facing.
/// Returns to Idle when the animation finishes (normalizedTime >= 1).
/// </summary>
public class AttackState : IPlayerState
{
    private readonly PlayerStateMachine m_Fsm;

    private static readonly int s_AttackHash = Animator.StringToHash("attack");

    public AttackState(PlayerStateMachine fsm)
    {
        m_Fsm = fsm;
    }

    public void Enter()
    {
        m_Fsm.Movement.BlockMovement();
        m_Fsm.Animation.LockFacing();
        m_Fsm.Animation.PlayAttack();
    }

    public void Update()
    {
        var stateInfo = m_Fsm.Animation.CurrentAnimState;

        // Attack clip finished — Animator won't leave this state on its own
        // (no transitions), so we check normalizedTime directly.
        if (stateInfo.shortNameHash == s_AttackHash && stateInfo.normalizedTime >= 1f)
        {
            m_Fsm.TransitionTo(PlayerStateType.Idle);
        }
    }

    public void Exit()
    {
        m_Fsm.Movement.UnblockMovement();
        m_Fsm.Animation.UnlockFacing();
    }
}
