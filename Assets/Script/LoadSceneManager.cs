using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Makes persistent objects survive scene loads, then handles GameScene reload.
/// </summary>
public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private string m_GameSceneName = "GameScene";
    [SerializeField] private UpgradePanel m_UpgradePanel;
    [SerializeField] private CountdownTimer m_Timer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        if (!SceneManager.GetSceneByName(m_GameSceneName).isLoaded)
            SceneManager.LoadScene(m_GameSceneName, LoadSceneMode.Additive);
    }

    public void ReloadGameScene()
    {
        if (m_Timer != null)
            m_Timer.ResetTimer();

        GameManager.IsGameOver = false;
        GameManager.HasStarted  = false;

        if (m_UpgradePanel != null)
            m_UpgradePanel.ClosePanel();

        // Sync BEFORE load so new scene sees fresh values from the start
        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();

        SceneManager.LoadScene(m_GameSceneName);
    }
}
