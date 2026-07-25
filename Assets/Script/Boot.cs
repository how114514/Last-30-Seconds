using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

/// <summary>
/// Entry point: loads Persistent + GameScene, then unloads itself.
/// </summary>
public class Boot : MonoBehaviour
{
    [SerializeField] private AssetReference m_PersistentScene;
    [SerializeField] private AssetReference m_GameScene;

    private async void Start()
    {
        Scene bootScene = SceneManager.GetActiveScene();

        // 1. Load Persistent
        if (m_PersistentScene != null)
        {
            var handle = m_PersistentScene.LoadSceneAsync(LoadSceneMode.Additive);
            await handle.Task;
        }

        // 2. Load GameScene
        if (m_GameScene != null)
        {
            var handle = m_GameScene.LoadSceneAsync(LoadSceneMode.Additive);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                SceneManager.SetActiveScene(handle.Result.Scene);
        }

        // 3. Unload Boot scene
        var unloadOp = SceneManager.UnloadSceneAsync(bootScene);
        if (unloadOp != null)
            await Awaitable.FromAsyncOperation(unloadOp);
    }
}
