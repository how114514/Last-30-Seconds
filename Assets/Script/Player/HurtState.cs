using UnityEngine;

/// <summary>
/// Hurt state: plays hurt animation, locks facing. Movement cancels into Move.
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
        m_Fsm.Animation.UnlockFacing();
    }
}
