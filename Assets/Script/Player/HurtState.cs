using UnityEngine;

/// <summary>
/// Hurt state: triggered when the player takes damage.
/// Plays hurt animation, blocks movement + facing,
/// returns to Idle when the animation finishes.
/// </summary>
public class HurtState : IPlayerState
{
    private readonly PlayerStateMachine m_Fsm;

    private static readonly int s_HurtHash = Animator.StringToHash("hurt");

    public HurtState(PlayerStateMachine fsm)
    {
        m_Fsm = fsm;
    }

    public void Enter()
    {
        m_Fsm.Movement.BlockMovement();
        m_Fsm.Animation.LockFacing();
        m_Fsm.Animation.PlayHurt();
    }

    public void Update()
    {
        var stateInfo = m_Fsm.Animation.CurrentAnimState;

        if (stateInfo.shortNameHash == s_HurtHash && stateInfo.normalizedTime >= 1f)
        {
            if (m_Fsm.HasAttackBuffered)
            {
                m_Fsm.ConsumeAttackBuffer();
                m_Fsm.TransitionTo(PlayerStateType.Attack);
            }
            else
            {
                m_Fsm.TransitionTo(PlayerStateType.Idle);
            }
        }
    }

    public void Exit()
    {
        m_Fsm.Movement.UnblockMovement();
        m_Fsm.Animation.UnlockFacing();
    }
}
