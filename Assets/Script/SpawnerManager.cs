using UnityEngine;

/// <summary>
/// Enables spawn points based on RuntimeData.enemySpawnPoints.
/// </summary>
public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] m_SpawnPoints = new GameObject[4];

    private void Start()
    {
        if (RuntimeData.Instance != null)
            ApplySpawnPoints(RuntimeData.Instance.enemySpawnPoints);
    }

    /// <summary>Enable the first 'count' spawn points, disable the rest.</summary>
    public void ApplySpawnPoints(int count)
    {
        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            if (m_SpawnPoints[i] != null)
                m_SpawnPoints[i].SetActive(i < count);
        }
    }
}
