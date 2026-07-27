using UnityEngine;

/// <summary>
/// Attack state: plays attack, locks facing. Movement cancels into Move.
/// Generates slash wave on animation end; buffers for chaining.
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
        m_Fsm.Animation.LockFacing();
        m_Fsm.Animation.PlayAttack();
        m_Fsm.Movement.SetSpeedOverride(2f);
    }

    public void Update()
    {
        var stateInfo = m_Fsm.Animation.CurrentAnimState;

        if (stateInfo.shortNameHash == s_AttackHash && stateInfo.normalizedTime >= 1f)
        {
            Player.Instance.SpawnSlashWave();

            if (m_Fsm.HasAttackBuffered)
            {
                m_Fsm.ConsumeAttackBuffer();
                m_Fsm.Animation.PlayAttack();
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
        m_Fsm.Movement.ClearSpeedOverride();
    }
}
