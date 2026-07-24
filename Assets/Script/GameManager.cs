using UnityEngine;

/// <summary>
/// Simple global game-state flag, set by the countdown timer.
/// </summary>
public static class GameManager
{
    public static bool HasStarted { get; set; }
    public static bool IsGameOver { get; set; }
}
