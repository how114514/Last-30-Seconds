using UnityEngine;

/// <summary>
/// End-of-run review stats. Separate from RuntimeData / UpgradeData.
/// </summary>
public static class GameStats
{
    // ── Live counters ────────────────────────────────────────────────

    public static int  EnemiesDefeatedThisRun { get; set; }
    public static int  TotalUpgradesPurchased { get; set; }
    public static float RunStartTime          { get; set; }
    public static float TotalStartTime        { get; set; }

    // ── Snapshot (captured at game end) ──────────────────────────────

    public static int   FinalScore            { get; private set; }
    public static int   EnemiesDefeated       { get; private set; }
    public static int   BossDefeated          { get; private set; }
    public static float TimePlayed            { get; private set; }
    public static int   MostEnemiesDefeated   { get; private set; }
    public static int   UpgradesPurchased     { get; private set; }

    // ── Methods ──────────────────────────────────────────────────────

    public static void OnEnemyKilled()
    {
        EnemiesDefeatedThisRun++;
    }

    public static void OnUpgradePurchased()
    {
        TotalUpgradesPurchased++;
    }

    public static void OnRunStart()
    {
        EnemiesDefeatedThisRun = 0;
        RunStartTime = Time.time;

        if (TotalStartTime <= 0f)
            TotalStartTime = Time.time;
    }

    /// <summary>Call at game end to snapshot current stats.</summary>
    public static void CaptureEndGame()
    {
        int roundScore = Player.Instance != null
            ? Player.Instance.GetComponent<PlayerScore>()?.CurrentScore ?? 0
            : 0;

        FinalScore += roundScore;

        EnemiesDefeated += EnemiesDefeatedThisRun;
        BossDefeated    = GameManager.TotalBossKills;
        TimePlayed      = Time.time - TotalStartTime;

        if (EnemiesDefeatedThisRun > MostEnemiesDefeated)
            MostEnemiesDefeated = EnemiesDefeatedThisRun;

        UpgradesPurchased = TotalUpgradesPurchased;
    }
}
