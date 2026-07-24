using TMPro;
using UnityEngine;

/// <summary>
/// Displays the player's score via TextMeshPro. Reads live from Player each frame.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text m_Text;

    private void Awake()
    {
        m_Text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        int score = 0;
        if (Player.Instance != null)
        {
            var ps = Player.Instance.GetComponent<PlayerScore>();
            if (ps != null) score = ps.CurrentScore;
        }
        m_Text.text = score.ToString();
    }
}
