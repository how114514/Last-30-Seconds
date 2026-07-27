using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Displays final game stats with labels and fade-in when enabled.
/// </summary>
public class VictoryPanel : MonoBehaviour
{
    [Header("Fade In")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private float m_FadeDuration = 0.5f;

    [Header("Exit")]
    [SerializeField] private float m_ExitDelay = 3f;
    [SerializeField] private TMP_Text m_ExitText;

    private float m_OpenTime;
    private bool m_CanExit;

    [Header("Stats Texts")]
    [SerializeField] private TMP_Text m_FinalScoreText;
    [SerializeField] private TMP_Text m_EnemiesDefeatedText;
    [SerializeField] private TMP_Text m_BossDefeatedText;
    [SerializeField] private TMP_Text m_TimePlayedText;
    [SerializeField] private TMP_Text m_MostEnemiesText;
    [SerializeField] private TMP_Text m_UpgradesPurchasedText;

    private void OnEnable()
    {
        if (m_FinalScoreText != null)
            m_FinalScoreText.text = $"Final Score\n{GameStats.FinalScore}";

        if (m_EnemiesDefeatedText != null)
            m_EnemiesDefeatedText.text = $"Enemies Defeated\n{GameStats.EnemiesDefeated}";

        if (m_BossDefeatedText != null)
            m_BossDefeatedText.text = $"Boss Defeated\n{GameStats.BossDefeated}";

        if (m_TimePlayedText != null)
        {
            int minutes = (int)(GameStats.TimePlayed / 60f);
            int seconds = (int)(GameStats.TimePlayed % 60f);
            string time = minutes > 0 ? $"{minutes}m {seconds}s" : $"{seconds}s";
            m_TimePlayedText.text = $"Time Played\n{time}";
        }

        if (m_MostEnemiesText != null)
            m_MostEnemiesText.text = $"Most Enemies In One Run\n{GameStats.MostEnemiesDefeated}";

        if (m_UpgradesPurchasedText != null)
            m_UpgradesPurchasedText.text = $"Upgrades Purchased\n{GameStats.UpgradesPurchased}";

        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.DOFade(1f, m_FadeDuration).SetUpdate(true);
        }

        m_OpenTime = Time.unscaledTime;
        m_CanExit = false;
    }

    private void Update()
    {
        float elapsed = Time.unscaledTime - m_OpenTime;
        float remaining = m_ExitDelay - elapsed;

        if (!m_CanExit)
        {
            if (remaining > 0f)
            {
                if (m_ExitText != null)
                    m_ExitText.text = Mathf.CeilToInt(remaining).ToString();
            }
            else
            {
                m_CanExit = true;
                if (m_ExitText != null)
                    m_ExitText.text = "Press Any Key To Continue";
            }
            return;
        }

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
            var lm = Object.FindFirstObjectByType<LoadSceneManager>();
            if (lm != null) lm.ReloadGameScene();
        }
    }
}
