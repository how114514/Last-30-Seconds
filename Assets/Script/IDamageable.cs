/// <summary>
/// Interface for anything that can take damage.
/// Implemented by PlayerScore, EnemyStats, etc.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}
