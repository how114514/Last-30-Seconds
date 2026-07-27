using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Finds boss by "Boss" tag. Enables fill parent when boss is alive.
/// Green fill updates instantly, red lags behind.
/// </summary>
public class BossHealthBar : MonoBehaviour
{
    [Header("Fill Parent")]
    [SerializeField] private GameObject m_FillParent;

    [Header("Fill Images")]
    [SerializeField] private Image m_GreenFill;
    [SerializeField] private Image m_RedFill;

    [Header("Animation")]
    [SerializeField] private float m_RedDelay = 0.3f;

    private EnemyStats m_Stats;
    private int m_PrevHealth;
    private Tweener m_RedTween;

    private void Start()
    {
        if (m_FillParent != null)
            m_FillParent.SetActive(false);
    }

    private void Update()
    {
        // Find boss if not tracked (or destroyed after scene reload)
        if (m_Stats == null)
        {
            var boss = GameObject.FindGameObjectWithTag("Boss");
            if (boss != null)
            {
                m_Stats = boss.GetComponent<EnemyStats>();
                m_PrevHealth = m_Stats != null ? m_Stats.CurrentHealth : 0;

                if (m_RedFill != null) m_RedFill.DOKill();
                float pct = m_Stats != null ? m_Stats.HealthPercent : 1f;
                if (m_GreenFill != null) m_GreenFill.fillAmount = pct;
                if (m_RedFill != null) m_RedFill.fillAmount = pct;

                if (m_FillParent != null)
                    m_FillParent.SetActive(m_Stats != null);
            }
            else
            {
                if (m_FillParent != null)
                    m_FillParent.SetActive(false);
            }
            return;
        }

        // Boss dead → hide bar
        if (m_Stats.IsDead)
        {
            m_Stats = null;
            if (m_FillParent != null)
                m_FillParent.SetActive(false);
            return;
        }

        float percent = m_Stats.HealthPercent;

        if (m_GreenFill != null)
            m_GreenFill.fillAmount = percent;

        if (m_RedFill != null && m_Stats.CurrentHealth < m_PrevHealth)
        {
            m_RedTween?.Kill();
            m_RedTween = m_RedFill.DOFillAmount(percent, m_RedDelay).SetEase(Ease.OutCubic);
        }

        m_PrevHealth = m_Stats.CurrentHealth;
    }
}
