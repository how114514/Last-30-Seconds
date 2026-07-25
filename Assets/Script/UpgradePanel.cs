using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 12 select buttons → show dimension info in shared display area.
/// Separate Upgrade button performs the actual purchase and level-up.
/// </summary>
public class UpgradePanel : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private UpgradeData m_UpgradeData;

    [Header("Scene")]
    [SerializeField] private LoadSceneManager m_LoadSceneManager;

    [Header("Currency")]
    [SerializeField] private TMP_Text m_CurrencyText;

    [Header("Shared Display")]
    [SerializeField] private TMP_Text m_NameText;
    [SerializeField] private TMP_Text m_DescriptionText;
    [SerializeField] private TMP_Text m_StageText;
    [SerializeField] private TMP_Text m_PriceText;

    [Header("Select Buttons")]
    [SerializeField] private Button[] m_SelectButtons = new Button[12];

    [Header("Action Buttons")]
    [SerializeField] private Button m_UpgradeButton;
    [SerializeField] private Button m_WithdrawButton;
    [SerializeField] private Button m_CloseButton;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_ClickSound;

    // Combined button index (spawnSlashWave → slashWaveDamage)
    private const int COMBINED_INDEX = 2;
    private const int ENEMY_COMBINED_INDEX = 11;

    private int m_SelectedIndex = -1;
    private UpgradeDimension m_SelectedDim;
    private int m_TotalCurrency;

    public int TotalCurrency => m_TotalCurrency;

    // ── Lifecycle ────────────────────────────────────────────────────

    private void OnEnable()
    {
        int score = 0;
        if (Player.Instance != null)
        {
            var ps = Player.Instance.GetComponent<PlayerScore>();
            if (ps != null) score = ps.CurrentScore;
        }

        m_TotalCurrency += score;
        UpdateCurrencyDisplay();

        WireSelectButtons();

        m_UpgradeButton?.onClick.RemoveAllListeners();
        m_UpgradeButton?.onClick.AddListener(OnUpgradeClicked);

        m_WithdrawButton?.onClick.RemoveAllListeners();
        m_WithdrawButton?.onClick.AddListener(OnWithdrawClicked);

        m_CloseButton?.onClick.RemoveAllListeners();
        m_CloseButton?.onClick.AddListener(OnCloseClicked);

        SelectButton(0);
    }

    private void PlayClick()
    {
        if (m_AudioSource != null && m_ClickSound != null)
            m_AudioSource.PlayOneShot(m_ClickSound);
    }

    // ── Currency ─────────────────────────────────────────────────────

    public bool Spend(int amount)
    {
        if (amount > m_TotalCurrency) return false;
        m_TotalCurrency -= amount;
        UpdateCurrencyDisplay();
        return true;
    }

    private void UpdateCurrencyDisplay()
    {
        if (m_CurrencyText != null)
            m_CurrencyText.text = $"${m_TotalCurrency}";
    }

    // ── Select Buttons ───────────────────────────────────────────────

    private void WireSelectButtons()
    {
        for (int i = 0; i < m_SelectButtons.Length; i++)
        {
            if (m_SelectButtons[i] == null) continue;
            int idx = i;
            m_SelectButtons[i].onClick.RemoveAllListeners();
            m_SelectButtons[i].onClick.AddListener(() => SelectButton(idx));
        }
    }

    private void SelectButton(int index)
    {
        PlayClick();
        m_SelectedIndex = index;
        m_SelectedDim = GetDimensionForIndex(index);
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (m_SelectedDim == null)
        {
            ClearDisplay();
            return;
        }

        bool isBoss = m_UpgradeData != null && m_SelectedDim == m_UpgradeData.enemyBoss;
        bool bossLocked = isBoss && GameManager.BossesDefeated < m_SelectedDim.currentLevel;
        bool isMaxed = m_SelectedDim.currentLevel >= m_SelectedDim.stages.Length - 1;
        int maxLevel = Mathf.Max(0, m_SelectedDim.stages.Length - 1);

        if (m_NameText != null)
            m_NameText.text = m_SelectedDim.dimensionName;

        if (m_DescriptionText != null)
            m_DescriptionText.text = m_SelectedDim.description;

        if (m_StageText != null)
            m_StageText.text = $"{m_SelectedDim.currentLevel}/{maxLevel}";

        if (m_PriceText != null)
        {
            if (bossLocked)
                m_PriceText.text = "Kill Boss";
            else if (isMaxed)
                m_PriceText.text = "MAX";
            else
                m_PriceText.text = $"${m_SelectedDim.stages[m_SelectedDim.currentLevel + 1].price}";
        }

        if (m_UpgradeButton != null)
            m_UpgradeButton.interactable = !isMaxed && !bossLocked;

        bool canWithdraw = m_SelectedIndex >= 8 && m_SelectedIndex <= 11;
        bool canWithdrawThis = canWithdraw && m_SelectedDim != null && (m_SelectedDim.currentLevel > 0 || (isBoss && m_SelectedDim.currentLevel == 0));
        if (m_WithdrawButton != null)
            m_WithdrawButton.interactable = canWithdrawThis;
    }

    private void ClearDisplay()
    {
        if (m_NameText != null)        m_NameText.text = "";
        if (m_DescriptionText != null) m_DescriptionText.text = "";
        if (m_StageText != null)       m_StageText.text = "";
        if (m_PriceText != null)       m_PriceText.text = "";
    }

    // ── Upgrade ──────────────────────────────────────────────────────

    private void OnUpgradeClicked()
    {
        if (m_SelectedDim == null) return;

        // Boss upgrade requires killing the previous boss
        if (m_SelectedDim == m_UpgradeData.enemyBoss
            && GameManager.BossesDefeated < m_SelectedDim.currentLevel) return;

        int nextLevel = m_SelectedDim.currentLevel + 1;
        if (nextLevel >= m_SelectedDim.stages.Length) return;

        int cost = m_SelectedDim.stages[nextLevel].price;
        if (!Spend(cost)) return;

        PlayClick();
        m_SelectedDim.currentLevel = nextLevel;

        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        // If combined button was at spawnSlashWave and we just unlocked it, re-resolve
        if (m_SelectedIndex == COMBINED_INDEX || m_SelectedIndex == ENEMY_COMBINED_INDEX)
            m_SelectedDim = GetDimensionForIndex(m_SelectedIndex);

        RefreshDisplay();
    }

    private void OnWithdrawClicked()
    {
        if (m_SelectedIndex < 8 || m_SelectedIndex > 11) return;
        if (m_SelectedDim == null) return;

        // Boss at 0 → switch back to variety at max-1
        if (m_SelectedDim == m_UpgradeData.enemyBoss && m_SelectedDim.currentLevel <= 0)
        {
            var variety = m_UpgradeData.enemyVariety;
            if (variety != null && variety.stages.Length > 1)
            {
                variety.currentLevel = variety.stages.Length - 2; // second-to-last
                if (RuntimeData.Instance != null) RuntimeData.Instance.SyncFromUpgradeData();
                m_SelectedDim = GetDimensionForIndex(m_SelectedIndex);
                PlayClick();
            }
            RefreshDisplay();
            return;
        }

        if (m_SelectedDim.currentLevel <= 0) return;

        PlayClick();
        m_SelectedDim.currentLevel--;

        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        if (m_SelectedIndex == COMBINED_INDEX || m_SelectedIndex == ENEMY_COMBINED_INDEX)
            m_SelectedDim = GetDimensionForIndex(m_SelectedIndex);

        RefreshDisplay();
    }

    private void OnCloseClicked()
    {
        PlayClick();
        if (m_LoadSceneManager != null)
            m_LoadSceneManager.ReloadGameScene();
    }

    /// <summary>Called by LoadSceneManager after scene finishes loading.</summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // ── Dimension Mapping ────────────────────────────────────────────

    private UpgradeDimension GetDimensionForIndex(int index)
    {
        if (m_UpgradeData == null) return null;

        return index switch
        {
            0  => m_UpgradeData.attackDamage,
            1  => m_UpgradeData.attackSpeed,
            2  => GetCombinedDimension(),
            3  => m_UpgradeData.slashWaveSize,
            4  => m_UpgradeData.movementSpeed,
            5  => m_UpgradeData.dodgeChance,
            6  => m_UpgradeData.resourceMultiplier,
            7  => m_UpgradeData.passiveIncome,
            8  => m_UpgradeData.enemySpawnInterval,
            9  => m_UpgradeData.enemySpawnCount,
            10 => m_UpgradeData.enemySpawnPoints,
            11 => GetEnemyCombinedDimension(),
            _  => null
        };
    }

    /// <summary>spawnSlashWave before unlock, slashWaveDamage after.</summary>
    private UpgradeDimension GetCombinedDimension()
    {
        var slash = m_UpgradeData.spawnSlashWave;
        bool unlocked = slash != null
                     && slash.currentLevel < slash.stages.Length
                     && slash.stages[slash.currentLevel].boolValue;

        return unlocked ? m_UpgradeData.slashWaveDamage : slash;
    }

    /// <summary>enemyVariety before max, enemyBoss after.</summary>
    private UpgradeDimension GetEnemyCombinedDimension()
    {
        var variety = m_UpgradeData.enemyVariety;
        if (variety == null) return null;

        bool maxed = variety.currentLevel >= variety.stages.Length - 1;
        return maxed ? m_UpgradeData.enemyBoss : variety;
    }
}
