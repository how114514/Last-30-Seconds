using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scrolls a UI Image vertically by cloning and looping.
/// </summary>
[RequireComponent(typeof(Image))]
public class UIScroller : MonoBehaviour
{
    [SerializeField] private float m_Speed = 50f;

    private Image m_Image;
    private Image m_Clone;
    private RectTransform m_CloneRect;
    private RectTransform m_Rect;
    private float m_Size;
    private float m_Offset;

    private void Start()
    {
        m_Image = GetComponent<Image>();
        m_Rect = GetComponent<RectTransform>();
        m_Size = m_Rect.rect.height;

        var cloneObj = new GameObject($"{name}_Clone", typeof(Image));
        cloneObj.transform.SetParent(transform.parent);
        cloneObj.transform.localScale = Vector3.one;

        m_Clone = cloneObj.GetComponent<Image>();
        m_Clone.sprite = m_Image.sprite;
        m_Clone.color = m_Image.color;
        m_Clone.type = m_Image.type;
        m_CloneRect = cloneObj.GetComponent<RectTransform>();
        m_CloneRect.sizeDelta = m_Rect.sizeDelta;
        m_CloneRect.anchorMin = m_Rect.anchorMin;
        m_CloneRect.anchorMax = m_Rect.anchorMax;
        m_CloneRect.pivot = m_Rect.pivot;
        m_CloneRect.anchoredPosition = m_Rect.anchoredPosition;
    }

    private void Update()
    {
        m_Offset += m_Speed * Time.deltaTime;
        if (m_Offset >= m_Size)
            m_Offset -= m_Size;

        Vector2 move = new Vector2(0, -m_Offset);
        Vector2 cloneOffset = new Vector2(0, m_Size);

        m_Rect.anchoredPosition = move;
        m_CloneRect.anchoredPosition = move + cloneOffset;
    }
}
