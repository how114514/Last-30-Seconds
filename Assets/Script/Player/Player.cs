using UnityEngine;

/// <summary>
/// Lightweight hub + singleton entry point.
/// </summary>
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimation))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private InputSystem_Actions m_InputActions;
    private PlayerStateMachine m_StateMachine;
    private PlayerMovement m_Movement;

    private void Awake()
    {
        Instance = this;

        m_InputActions = new InputSystem_Actions();
        m_StateMachine = GetComponent<PlayerStateMachine>();
        m_Movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        m_Movement.Initialize(m_InputActions);
        m_StateMachine.Initialize(m_InputActions);
    }

    private void OnEnable()
    {
        m_InputActions?.Enable();
    }

    private void OnDisable()
    {
        m_InputActions?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        m_InputActions?.Dispose();
    }
}
