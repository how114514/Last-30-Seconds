using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStateType { Idle, Move, Attack, Hurt }

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

/// <summary>
/// State machine: creates states on Awake, then Initialize() subscribes
/// to input and explicitly enters the Idle state. Nothing relies on
/// implicit Start timing.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimation))]
public class PlayerStateMachine : MonoBehaviour
{
    private readonly Dictionary<PlayerStateType, IPlayerState> m_States = new();
    private IPlayerState m_CurrentState;
    private PlayerStateType m_CurrentStateType;

    private PlayerMovement m_Movement;
    private PlayerAnimation m_Animation;
    private InputSystem_Actions m_InputActions;
    private bool m_Initialized;

    public PlayerStateType CurrentStateType => m_CurrentStateType;
    public PlayerMovement Movement => m_Movement;
    public PlayerAnimation Animation => m_Animation;
    public bool IsInitialized => m_Initialized;

    private void Awake()
    {
        m_Movement = GetComponent<PlayerMovement>();
        m_Animation = GetComponent<PlayerAnimation>();

        m_States[PlayerStateType.Idle]   = new IdleState(this);
        m_States[PlayerStateType.Move]   = new MoveState(this);
        m_States[PlayerStateType.Attack] = new AttackState(this);
        m_States[PlayerStateType.Hurt]   = new HurtState(this);
    }

    /// <summary>
    /// Called by Player.Start — wires input and explicitly enters Idle.
    /// By this point all component Awakes have run, so cross-references are safe.
    /// </summary>
    public void Initialize(InputSystem_Actions inputActions)
    {
        if (m_Initialized) return;

        m_InputActions = inputActions;
        m_InputActions.Player.Attack.performed += OnAttackPerformed;

        // Explicitly enter the default state
        m_CurrentStateType = PlayerStateType.Idle;
        m_CurrentState = m_States[PlayerStateType.Idle];
        m_CurrentState.Enter();

        m_Initialized = true;
    }

    private void Update()
    {
        if (!m_Initialized) return;
        m_CurrentState?.Update();
    }

    private void OnDestroy()
    {
        if (m_InputActions != null)
            m_InputActions.Player.Attack.performed -= OnAttackPerformed;
    }

    public void TransitionTo(PlayerStateType type)
    {
        if (!m_Initialized) return;
        if (m_CurrentStateType == type) return;

        m_CurrentState?.Exit();
        m_CurrentStateType = type;
        m_CurrentState = m_States[type];
        m_CurrentState?.Enter();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (m_CurrentStateType == PlayerStateType.Idle || m_CurrentStateType == PlayerStateType.Move)
        {
            TransitionTo(PlayerStateType.Attack);
        }
    }
}
