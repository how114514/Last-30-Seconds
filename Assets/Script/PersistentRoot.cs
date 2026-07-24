using UnityEngine;

/// <summary>
/// Put on the root GameObject of the Persistent scene.
/// Everything under it survives scene loads.
/// </summary>
public class PersistentRoot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
