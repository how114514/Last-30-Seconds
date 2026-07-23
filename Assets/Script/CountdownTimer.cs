using TMPro;
using UnityEngine;

/// <summary>
/// Countdown timer displayed via TextMeshPro.
/// Format: M:SS.mmm. Flashes red in the last 5 seconds.
/// Pauses the game when time runs out.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class CountdownTimer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_StartTime = 30f;
    [SerializeField] private float m_FlashThreshold = 5f;
    [SerializeField] private float m_FlashInterval = 0.25f;

    private TMP_Text m_Text;
    private float m_Remaining;
    private float m_FlashTimer;
    private bool m_Running = true;

    private static readonly Color k_NormalColor = Color.white;
    private static readonly Color k_FlashColor  = new(1f, 0.2f, 0.2f, 1f);

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
        m_Remaining = m_StartTime;
        m_Text.color = k_NormalColor;
    }

    private void Update()
    {
        if (!m_Running) return;

        m_Remaining -= Time.deltaTime;

        if (m_Remaining <= 0f)
        {
            m_Remaining = 0f;
            m_Running = false;
            m_Text.color = k_FlashColor;
            Time.timeScale = 0f;
        }
        else if (m_Remaining <= m_FlashThreshold)
        {
            m_FlashTimer += Time.deltaTime;
            m_Text.color = (int)(m_FlashTimer / m_FlashInterval) % 2 == 0
                ? k_FlashColor
                : k_NormalColor;
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
