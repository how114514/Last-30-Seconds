using UnityEngine;

/// <summary>
/// Player score (health). Starts at 0, cannot drop below 0, never dies.
/// Triggers Hurt state when taking damage.
/// </summary>
public class PlayerScore : MonoBehaviour, IDamageable
{
    [SerializeField] private int m_CurrentScore;

    private float m_ScoreTimer;

    private float DodgeRate => RuntimeData.Instance != null ? RuntimeData.Instance.dodgeChance : 0f;
    private float ResourceMultiplier => RuntimeData.Instance != null ? RuntimeData.Instance.resourceMultiplier : 1f;

    private PlayerStateMachine m_StateMachine;

    public int CurrentScore => m_CurrentScore;

    private void Awake()
    {
        m_StateMachine = GetComponent<PlayerStateMachine>();
    }

    private void Update()
    {
        if (!GameManager.HasStarted) return;

        m_ScoreTimer += Time.deltaTime;
        if (m_ScoreTimer >= 1f)
        {
            m_ScoreTimer -= 1f;
            int passive = RuntimeData.Instance != null ? RuntimeData.Instance.passiveIncome : 0;
            m_CurrentScore += Mathf.RoundToInt(passive * ResourceMultiplier);
        }
    }

    /// <summary>Called when killing an enemy — increases the score.</summary>
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        m_CurrentScore += Mathf.RoundToInt(amount * ResourceMultiplier);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        if (Random.value < DodgeRate) return;

        m_CurrentScore = Mathf.Max(0, m_CurrentScore - amount);

        if (m_StateMachine != null)
        {
            m_StateMachine.TransitionTo(PlayerStateType.Hurt);
        }
    }
}
