using UnityEngine;

/// <summary>
/// Lightweight hub + singleton entry point. Also holds slash-wave spawn config.
/// </summary>
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimation))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Slash Wave")]
    [SerializeField] private bool m_SpawnSlashWave = true;
    [SerializeField] private GameObject m_SlashWavePrefab;
    [SerializeField] private Transform m_SlashWaveSpawnPoint;
    [SerializeField] private float m_SlashWaveScaleX = 1f;
    [SerializeField] private int m_SlashWaveDamage = 10;

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

    private void OnEnable()  => m_InputActions?.Enable();
    private void OnDisable() => m_InputActions?.Disable();

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        m_InputActions?.Dispose();
    }

    /// <summary>Spawn and configure a slash wave.</summary>
    public void SpawnSlashWave()
    {
        if (!m_SpawnSlashWave || m_SlashWavePrefab == null) return;

        var pos = m_SlashWaveSpawnPoint != null ? m_SlashWaveSpawnPoint.position : transform.position;
        var wave = Instantiate(m_SlashWavePrefab, pos, Quaternion.identity);
        var scale = wave.transform.localScale;
        scale.x = m_SlashWaveScaleX * Mathf.Sign(transform.localScale.x);
        wave.transform.localScale = scale;

        var dealer = wave.GetComponent<DamageDealer>();
        if (dealer != null) dealer.SetDamage(m_SlashWaveDamage);

        var waveScript = wave.GetComponent<SlashWave>();
        if (waveScript != null) waveScript.SelectAnimation(m_SlashWaveDamage);
    }
}
