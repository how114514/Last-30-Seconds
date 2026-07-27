using UnityEngine;

/// <summary>
/// Global game state and boss progression.
/// </summary>
public static class GameManager
{
    public static bool HasStarted { get; set; }
    public static bool IsGameOver { get; set; }
    public static bool BossAlreadyDead { get; set; }

    /// <summary>How many unique boss levels the player has beaten (0-3).</summary>
    public static int BossesDefeated { get; private set; }

    /// <summary>Total number of boss kills (not unique).</summary>
    public static int TotalBossKills { get; private set; }

    /// <summary>Called when a boss is killed.</summary>
    public static void BossDefeated()
    {
        TotalBossKills++;

        int bossLevel = RuntimeData.Instance != null ? RuntimeData.Instance.enemyBoss : 0;
        if (bossLevel > BossesDefeated)
            BossesDefeated = bossLevel;

        BossAlreadyDead = true;

        var timer = Object.FindFirstObjectByType<CountdownTimer>();

        if (BossesDefeated >= 3)
        {
            timer?.BossWin();
        }
        else
        {
            timer?.ForceEnd();
        }
    }
}
