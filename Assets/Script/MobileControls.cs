using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Mobile touch buttons for move left/right and attack.
/// Multi-touch via pointer ID tracking.
/// </summary>
public class MobileControls : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool m_IsMobile;

    [Header("Buttons")]
    [SerializeField] private Button m_MoveLeftButton;
    [SerializeField] private Button m_MoveRightButton;
    [SerializeField] private Button m_AttackButton;

    public static MobileControls Instance { get; private set; }

    public bool LeftHeld { get; private set; }
    public bool RightHeld { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool IsMobile => m_IsMobile;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!m_IsMobile) return;

        BindHold(m_MoveLeftButton,  val => LeftHeld  = val);
        BindHold(m_MoveRightButton, val => RightHeld = val);
        BindTap(m_AttackButton,  () => AttackPressed = true);
    }

    private void LateUpdate()
    {
        if (!m_IsMobile) return;

        bool panelOpen = UpgradePanelIsOpen();
        SetActive(m_MoveLeftButton,  !panelOpen);
        SetActive(m_MoveRightButton, !panelOpen);
        SetActive(m_AttackButton,    !panelOpen);
    }

    public void ConsumeAttack() => AttackPressed = false;

    private bool UpgradePanelIsOpen()
    {
        var panel = Object.FindFirstObjectByType<UpgradePanel>();
        return panel != null && panel.gameObject.activeSelf;
    }

    private void SetActive(Button btn, bool active)
    {
        if (btn != null) btn.gameObject.SetActive(active);
    }

    private void BindHold(Button btn, System.Action<bool> setter)
    {
        if (btn == null) return;
        var h = btn.gameObject.AddComponent<MultiTouchHandler>();
        int id = -1;
        h.OnDownId += (pointerId, _) =>
        {
            if (id < 0) { id = pointerId; setter(true); }
        };
        h.OnUpId += (pointerId, _) =>
        {
            if (pointerId == id) { id = -1; setter(false); }
        };
    }

    private void BindTap(Button btn, System.Action action)
    {
        if (btn == null) return;
        var h = btn.gameObject.AddComponent<MultiTouchHandler>();
        h.OnDownId += (_, _) => action();
    }
}

/// <summary>
/// Per-button multi-touch handler via pointer IDs.
/// </summary>
public class MultiTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public event System.Action<int, PointerEventData> OnDownId;
    public event System.Action<int, PointerEventData> OnUpId;

    private int m_PointerId = -1;

    public void OnPointerDown(PointerEventData d)
    {
        if (m_PointerId >= 0) return;
        m_PointerId = d.pointerId;
        OnDownId?.Invoke(d.pointerId, d);
    }

    public void OnPointerUp(PointerEventData d)
    {
        if (d.pointerId != m_PointerId) return;
        m_PointerId = -1;
        OnUpId?.Invoke(d.pointerId, d);
    }

    public void OnPointerExit(PointerEventData d)
    {
        if (d.pointerId != m_PointerId) return;
        m_PointerId = -1;
        OnUpId?.Invoke(d.pointerId, d);
    }
}
