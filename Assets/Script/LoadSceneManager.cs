using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages GameScene loading with fade transitions.
/// </summary>
public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private string m_GameSceneName = "GameScene";
    [SerializeField] private UpgradePanel m_UpgradePanel;
    [SerializeField] private CountdownTimer m_Timer;
    [SerializeField] private FadePanel m_FadePanel;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        if (!SceneManager.GetSceneByName(m_GameSceneName).isLoaded)
        {
            SceneManager.LoadScene(m_GameSceneName, LoadSceneMode.Additive);
            m_FadePanel?.FadeIn();
        }
    }

    public void ReloadGameScene()
    {
        if (m_Timer != null)
            m_Timer.ResetTimer();

        GameManager.IsGameOver = false;
        GameManager.HasStarted  = false;

        if (m_UpgradePanel != null)
            m_UpgradePanel.ClosePanel();

        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        if (m_FadePanel != null)
        {
            m_FadePanel.FadeOut().OnComplete(() =>
            {
                SceneManager.LoadScene(m_GameSceneName);
                m_FadePanel.FadeIn();
            });
        }
        else
        {
            SceneManager.LoadScene(m_GameSceneName);
        }
    }
}
