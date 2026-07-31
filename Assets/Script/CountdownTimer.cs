using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Countdown timer displayed via TextMeshPro.
/// Format: M:SS.mmm. Flashes red in the last 5 seconds.
/// When time runs out: stops the game and opens the upgrade panel.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CountdownTimer : MonoBehaviour
{
    [Header("End Game")]
    [SerializeField] private GameObject m_UpgradePanel;
    [SerializeField] private GameObject m_VictoryPanel;

    [Header("Settings")]
    [SerializeField] private float m_StartTime = 30f;
    [SerializeField] private float m_FlashThreshold = 5f;
    [SerializeField] private float m_FlashInterval = 0.25f;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_BeepSound;
    [SerializeField] private float m_BeepOffset = 100f;

    private TMP_Text m_Text;
    private float m_Remaining;
    private float m_FlashTimer;
    private int m_PrevSecond;
    private bool m_Running = true;

    private static readonly Color k_NormalColor = Color.black;
    private static readonly Color k_FlashColor  = new(1f, 0.2f, 0.2f, 1f);

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
        ResetTimer();
    }

    /// <summary>Reset to initial state, ready for a new round.</summary>
    public void ResetTimer()
    {
        m_Remaining = m_StartTime;
        m_Running = false;
        m_FlashTimer = 0f;
        m_PrevSecond = Mathf.CeilToInt(m_StartTime);
        m_Text.color = k_NormalColor;
        UpdateDisplay();
    }

    /// <summary>Boss 1/2 killed — force timer to 0 so it triggers EndGame naturally.</summary>
    public void ForceEnd()
    {
        m_Remaining = 0f;
        m_Running = false;
        m_Text.color = k_NormalColor;
        UpdateDisplay();
        EndGame();
    }

    /// <summary>Boss 3 killed — game victory.</summary>
    public void BossWin()
    {
        GameStats.CaptureEndGame();
        m_Running = false;
        m_Text.color = k_NormalColor;
        m_Remaining = 0f;
        UpdateDisplay();

        GameManager.IsGameOver = true;

        if (Player.Instance != null)
        {
            var fsm = Player.Instance.GetComponent<PlayerStateMachine>();
            if (fsm != null) fsm.OnGameEnd();
        }

        if (m_VictoryPanel != null)
            m_VictoryPanel.SetActive(true);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var stats = enemy.GetComponent<EnemyStats>();
            if (stats != null) stats.DieSilently();
            else Destroy(enemy);
        }
    }

    private void Update()
    {
        if (!m_Running)
        {
            bool anyKey = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool mobileTap = MobileControls.Instance != null && MobileControls.Instance.IsMobile && MobileControls.Instance.AttackPressed;
            if (!GameManager.IsGameOver && (anyKey || mobileTap))
            {
                if (mobileTap) MobileControls.Instance.ConsumeAttack();
                m_Running = true;
                GameManager.HasStarted = true;
                GameStats.OnRunStart();
            }
            return;
        }

        float dt = Time.unscaledDeltaTime;
        m_Remaining -= dt;

        if (m_Remaining <= 0f)
        {
            m_Remaining = 0f;
            m_Running = false;
            m_Text.color = k_FlashColor;
            EndGame();
            return;
        }

        // Beep at seconds 5,4,3,2,1 (offset to account for audio latency)
        float adjusted = m_Remaining - m_BeepOffset;
        int currentSecond = Mathf.FloorToInt(adjusted);

        if (currentSecond != m_PrevSecond)
        {
            m_PrevSecond = currentSecond;
            if (currentSecond >= 0 && currentSecond < m_FlashThreshold
                && m_AudioSource != null && m_BeepSound != null)
            {
                m_AudioSource.PlayOneShot(m_BeepSound);
            }
        }

        if (m_Remaining <= m_FlashThreshold)
        {
            m_FlashTimer += dt;
            m_Text.color = (int)(m_FlashTimer / m_FlashInterval) % 2 == 0
                ? k_FlashColor
                : k_NormalColor;
        }

        UpdateDisplay();
    }

    private void EndGame()
    {
        GameStats.CaptureEndGame();
        GameManager.IsGameOver = true;

        if (Player.Instance != null)
        {
            var fsm = Player.Instance.GetComponent<PlayerStateMachine>();
            if (fsm != null) fsm.OnGameEnd();
        }

        if (m_UpgradePanel != null)
            m_UpgradePanel.SetActive(true);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
                stats.DieSilently();
            else
                Destroy(enemy);
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int minutes = (int)(m_Remaining / 60f);
        float seconds = m_Remaining % 60f;
        m_Text.text = $"{minutes}:{seconds:00.000}";
    }
}
