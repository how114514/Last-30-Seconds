/// <summary>
/// Move state: plays move animation, flips sprite, and applies movement.
/// Transitions back to Idle when movement stops.
/// </summary>
public class MoveState : IPlayerState
{
    private readonly PlayerStateMachine m_Fsm;

    public MoveState(PlayerStateMachine fsm)
    {
        m_Fsm = fsm;
    }

    public void Enter()
    {
        m_Fsm.Animation.PlayMove();
    }

    public void Update()
    {
        // Flip sprite based on horizontal input
        var moveInput = m_Fsm.Movement.MoveInput;
        m_Fsm.Animation.ApplyFacing(moveInput.x);

        // Go back to idle when input stops
        if (!m_Fsm.Movement.IsMoving)
        {
            m_Fsm.TransitionTo(PlayerStateType.Idle);
        }
    }

    public void Exit() { }
}
