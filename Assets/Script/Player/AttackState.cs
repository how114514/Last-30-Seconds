using UnityEngine;

/// <summary>
/// Attack state: plays attack animation, blocks movement + facing.
/// When animation finishes: if a buffered attack was pressed, re-enter Attack;
/// otherwise return to Idle.
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

        if (stateInfo.shortNameHash == s_AttackHash && stateInfo.normalizedTime >= 1f)
        {
            // Animation finished → always spawn slash wave
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
        m_Fsm.Movement.UnblockMovement();
        m_Fsm.Animation.UnlockFacing();
    }
}
