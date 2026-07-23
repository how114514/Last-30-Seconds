using UnityEngine;

/// <summary>
/// Idle state: plays idle animation, watches for movement input to transition to Move.
/// </summary>
public class IdleState : IPlayerState
{
    private readonly PlayerStateMachine m_Fsm;

    public IdleState(PlayerStateMachine fsm)
    {
        m_Fsm = fsm;
    }

    public void Enter()
    {
        m_Fsm.Animation.PlayIdle();
    }

    public void Update()
    {
        if (m_Fsm.Movement.IsMoving)
        {
            m_Fsm.TransitionTo(PlayerStateType.Move);
        }
    }

    public void Exit() { }
}
