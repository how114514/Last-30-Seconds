using TMPro;
using UnityEngine;

/// <summary>
/// Displays the player's score via TextMeshPro, updated every frame.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text m_Text;
    private PlayerScore m_Score;

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (Player.Instance != null)
            m_Score = Player.Instance.GetComponent<PlayerScore>();
    }

    private void Update()
    {
        if (m_Score != null)
            m_Text.text = m_Score.CurrentScore.ToString();
    }
}
