using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Pulsating prompt. Hides itself when game starts.
/// </summary>
public class StartPrompt : MonoBehaviour
{
    [Header("Scale Animation")]
    [SerializeField] private float m_MinScale = 0.9f;
    [SerializeField] private float m_MaxScale = 1.1f;
    [SerializeField] private float m_Duration = 0.6f;

    private void Start()
    {
        transform.DOScale(m_MaxScale, m_Duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    private void Update()
    {
        if (GameManager.HasStarted)
            gameObject.SetActive(false);
    }
}
