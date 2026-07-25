using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls regular spawn points and boss spawning based on RuntimeData.
/// If enemyBoss > 0, enables boss spawner and disables normal spawners.
/// </summary>
public class SpawnerManager : MonoBehaviour
{
    [Header("Normal Spawners")]
    [SerializeField] private GameObject[] m_SpawnPoints = new GameObject[4];

    [Header("Boss")]
    [SerializeField] private BossSpawner m_BossSpawner;
    [SerializeField] private List<EnemyData> m_BossDataList; // index 0→1, 1→2, 2→3

    private void Start()
    {
        if (RuntimeData.Instance == null) return;

        int bossLevel = RuntimeData.Instance.enemyBoss;

        if (bossLevel > 0)
        {
            // Boss mode: disable normal spawners, enable boss
            SetSpawnersActive(false);

            if (m_BossSpawner != null)
            {
                int dataIndex = Mathf.Clamp(bossLevel - 1, 0, m_BossDataList.Count - 1);
                if (dataIndex < m_BossDataList.Count && m_BossDataList[dataIndex] != null)
                {
                    m_BossSpawner.SetBossData(m_BossDataList[dataIndex]);
                    m_BossSpawner.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Normal mode
            if (m_BossSpawner != null)
                m_BossSpawner.gameObject.SetActive(false);

            ApplySpawnPoints(RuntimeData.Instance.enemySpawnPoints);
        }
    }

    private void SetSpawnersActive(bool active)
    {
        foreach (var sp in m_SpawnPoints)
        {
            if (sp != null) sp.SetActive(active);
        }
    }

    public void ApplySpawnPoints(int count)
    {
        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            if (m_SpawnPoints[i] != null)
                m_SpawnPoints[i].SetActive(i < count);
        }
    }
}
