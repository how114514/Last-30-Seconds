using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades a full-screen Image in/out using DOTween. Call FadeOut / FadeIn.
/// </summary>
[RequireComponent(typeof(Image))]
public class FadePanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_FadeDuration = 0.5f;

    private Image m_Image;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Fade to black. Returns the tween so callers can wait on it.</summary>
    public Tween FadeOut()
    {
        m_Image.raycastTarget = true;
        return m_Image.DOFade(1f, m_FadeDuration).SetUpdate(true);
    }

    /// <summary>Fade to clear. Returns the tween so callers can wait on it.</summary>
    public Tween FadeIn()
    {
        return m_Image.DOFade(0f, m_FadeDuration)
            .SetUpdate(true)
            .OnComplete(() => m_Image.raycastTarget = false);
    }
}
