using UnityEngine;

/// <summary>
/// Singleton holding runtime player/enemy data. Fields are visible in the inspector
/// for debugging, but are auto-populated from UpgradeData on Start.
/// </summary>
[DefaultExecutionOrder(-100)]
public class RuntimeData : MonoBehaviour
{
    public static RuntimeData Instance { get; private set; }

    [Header("Source")]
    [SerializeField] private UpgradeData m_UpgradeData;

    [Header("Combat")]
    public int     attackDamage       = 10;
    public float   attackSpeed        = 1f;
    public bool    spawnSlashWave;
    public int     slashWaveDamage    = 10;
    public float   slashWaveSize      = 1f;

    [Header("Movement")]
    public float   movementSpeed      = 5f;

    [Header("Defense")]
    public float   dodgeChance;

    [Header("Economy")]
    public float   resourceMultiplier = 1f;
    public int     passiveIncome      = 1;

    [Header("Enemy")]
    public float   enemySpawnInterval = 3f;
    public int     enemySpawnCount    = 1;
    public int     enemySpawnPoints   = 3;
    public int     enemyVariety       = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SyncFromUpgradeData();
    }

    /// <summary>Re-read all values from UpgradeData. Call on each game start.</summary>
    public void SyncFromUpgradeData()
    {
        if (m_UpgradeData == null) return;

        attackDamage       = GetInt(m_UpgradeData.attackDamage);
        attackSpeed        = GetFloat(m_UpgradeData.attackSpeed);
        spawnSlashWave     = GetBool(m_UpgradeData.spawnSlashWave);
        slashWaveDamage    = GetInt(m_UpgradeData.slashWaveDamage);
        slashWaveSize      = GetFloat(m_UpgradeData.slashWaveSize);
        movementSpeed      = GetFloat(m_UpgradeData.movementSpeed);
        dodgeChance        = GetFloat(m_UpgradeData.dodgeChance);
        resourceMultiplier = GetFloat(m_UpgradeData.resourceMultiplier);
        passiveIncome      = GetInt(m_UpgradeData.passiveIncome);
        enemySpawnInterval = GetFloat(m_UpgradeData.enemySpawnInterval);
        enemySpawnCount    = GetInt(m_UpgradeData.enemySpawnCount);
        enemySpawnPoints   = GetInt(m_UpgradeData.enemySpawnPoints);
        enemyVariety       = GetInt(m_UpgradeData.enemyVariety);
    }

    private int   GetInt(UpgradeDimension d)   => GetStage(d)?.intValue   ?? 0;
    private float GetFloat(UpgradeDimension d) => GetStage(d)?.floatValue ?? 0f;
    private bool  GetBool(UpgradeDimension d)  => GetStage(d)?.boolValue  ?? false;

    private UpgradeStage GetStage(UpgradeDimension d)
    {
        if (d == null || d.stages == null || d.stages.Length == 0) return null;
        int i = Mathf.Clamp(d.currentLevel, 0, d.stages.Length - 1);
        return d.stages[i];
    }
}
