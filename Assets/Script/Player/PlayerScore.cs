using UnityEngine;

/// <summary>
/// Player score (health). Starts at 0, cannot drop below 0, never dies.
/// Triggers Hurt state when taking damage.
/// </summary>
public class PlayerScore : MonoBehaviour, IDamageable
{
    [SerializeField] private int m_CurrentScore;

    [Header("Dodge")]
    [SerializeField] private float m_DodgeRate;

    private PlayerStateMachine m_StateMachine;

    public int CurrentScore => m_CurrentScore;

    private void Awake()
    {
        m_StateMachine = GetComponent<PlayerStateMachine>();
    }

    /// <summary>Called when killing an enemy — increases the score.</summary>
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        m_CurrentScore += amount;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        if (Random.value < m_DodgeRate) return;

        m_CurrentScore = Mathf.Max(0, m_CurrentScore - amount);

        if (m_StateMachine != null)
        {
            m_StateMachine.TransitionTo(PlayerStateType.Hurt);
        }
    }
}
