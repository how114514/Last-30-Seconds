using UnityEngine;

public enum DimensionType { Float, Int, Bool }

/// <summary>
/// One upgrade stage. Use the field matching the dimension's type.
/// </summary>
[System.Serializable]
public class UpgradeStage
{
    public float floatValue;
    public int   intValue;
    public bool  boolValue;
    public int   price; 
}

/// <summary>
/// One upgrade dimension with shared name/desc, type, current progress, and 21 stages.
/// </summary>
[System.Serializable]
public class UpgradeDimension
{
    public string dimensionName;
    [TextArea]
    public string description;
    public DimensionType type;
    public int currentLevel;
    public UpgradeStage[] stages = new UpgradeStage[1];
}

/// <summary>
/// Holds upgrade progression for all RuntimeData dimensions.
/// enemySpawnPoints / enemyVariety are left for later.
/// </summary>
[CreateAssetMenu(fileName = "UpgradeData", menuName = "Last 30 Seconds/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Combat")]
    public UpgradeDimension attackDamage;      // Int
    public UpgradeDimension attackSpeed;       // Float
    public UpgradeDimension spawnSlashWave;    // Bool
    public UpgradeDimension slashWaveDamage;   // Int
    public UpgradeDimension slashWaveSize;     // Float

    [Header("Movement")]
    public UpgradeDimension movementSpeed;     // Float

    [Header("Defense")]
    public UpgradeDimension dodgeChance;       // Float

    [Header("Economy")]
    public UpgradeDimension resourceMultiplier; // Float
    public UpgradeDimension passiveIncome;      // Int

    [Header("Enemy")]
    public UpgradeDimension enemySpawnInterval; // Float
    public UpgradeDimension enemySpawnCount;    // Int
    public UpgradeDimension enemySpawnPoints;   // Int
    public UpgradeDimension enemyVariety;       // Int
    public UpgradeDimension enemyBoss;          // Int
}
