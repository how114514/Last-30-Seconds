using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads/unloads GameScene via Addressables.
/// Load-before-unload: avoids the blank frame between scenes.
/// </summary>
public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private string m_GameSceneKey = "GameScene";
    [SerializeField] private UpgradePanel m_UpgradePanel;
    [SerializeField] private CountdownTimer m_Timer;
    [SerializeField] private FadePanel m_FadePanel;

    public static LoadSceneManager Instance { get; private set; }

    private bool m_IsLoading;

    public bool IsLoading => m_IsLoading;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (RuntimeData.Instance != null)
            RuntimeData.Instance.SyncFromUpgradeData();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ── Delayed boss kill reload ─────────────────────────────────────

    public void DelayedReload()
    {
        StartCoroutine(DelayedReloadRoutine());
    }

    private IEnumerator DelayedReloadRoutine()
    {
        yield return new WaitForSeconds(0.02f);
        if (GameManager.BossAlreadyDead) yield break;
        ReloadGameScene();
    }

    // ── Public reload ────────────────────────────────────────────────

    public void ReloadGameScene()
    {
        if (m_IsLoading) return;

        if (m_Timer != null) m_Timer.ResetTimer();
        GameManager.IsGameOver = false;
        GameManager.HasStarted  = false;
        GameManager.BossAlreadyDead = false;

        if (MobileControls.Instance != null)
            MobileControls.Instance.ResetAll();
        if (m_UpgradePanel != null) m_UpgradePanel.ClosePanel();

        if (RuntimeData.Instance != null) RuntimeData.Instance.SyncFromUpgradeData();

        if (m_FadePanel != null)
        {
            m_IsLoading = true;
            m_FadePanel.FadeOut().OnComplete(() => ExecuteReload());
        }
        else
        {
            ExecuteReload();
        }
    }

    private void ExecuteReload()
    {
        Scene previousActive = SceneManager.GetActiveScene();

        var loadHandle = Addressables.LoadSceneAsync(m_GameSceneKey, LoadSceneMode.Additive);
        loadHandle.Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                SceneManager.SetActiveScene(handle.Result.Scene);

                var unloadOp = SceneManager.UnloadSceneAsync(previousActive);
                if (unloadOp != null)
                    unloadOp.completed += _ => OnReloadComplete();
                else
                    OnReloadComplete();
            }
            else
            {
                m_IsLoading = false;
                Debug.LogError($"[LoadSceneManager] Failed to load: {m_GameSceneKey}");
            }
        };
    }

    private void EnsureEventSystem()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void OnReloadComplete()
    {
        EnsureEventSystem();

        if (m_FadePanel != null)
            m_FadePanel.FadeIn().OnComplete(() => m_IsLoading = false);
        else
            m_IsLoading = false;
    }
}
