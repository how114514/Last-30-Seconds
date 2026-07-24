using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Pulsating "press any key" prompt. Disables itself when the game starts.
/// </summary>
public class StartPrompt : MonoBehaviour
{
    [Header("Scale Animation")]
    [SerializeField] private float m_MinScale = 0.9f;
    [SerializeField] private float m_MaxScale = 1.1f;
    [SerializeField] private float m_Duration = 0.6f;

    private TMP_Text m_Text;

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
        m_Text.text = "Press Any Key To Start";
    }

    private void Start()
    {
        // Pulse the scale back and forth
        transform.DOScale(m_MaxScale, m_Duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    private void Update()
    {
        if (GameManager.HasStarted)
        {
            transform.DOKill();
            gameObject.SetActive(false);
        }
    }
}
