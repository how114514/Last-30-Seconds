using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Periodically spawns enemies from a weighted random pool.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject m_Prefab;

    [Header("Data Pool")]
    [SerializeField] private List<EnemyData> m_DataList;

    [Header("Spawn Settings")]
    [SerializeField] private float m_SpawnInterval = 3f;
    [SerializeField] private int m_SpawnCount = 1;

    [Header("Spawn Point")]
    [SerializeField] private Transform m_SpawnPoint;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            for (int i = 0; i < m_SpawnCount; i++)
            {
                Spawn();
                if (m_SpawnCount > 1)
                    yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(m_SpawnInterval);
        }
    }

    /// <summary>Spawn one randomly-picked enemy.</summary>
    public GameObject Spawn()
    {
        if (m_Prefab == null)
        {
            Debug.LogWarning("EnemySpawner: no prefab assigned.", this);
            return null;
        }

        var data = PickRandom();
        if (data == null)
        {
            Debug.LogWarning("EnemySpawner: no EnemyData in list.", this);
            return null;
        }

        var pos = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
        var enemy = Instantiate(m_Prefab, pos, Quaternion.identity);

        var stats = enemy.GetComponent<EnemyStats>();
        if (stats != null)
            stats.ApplyData(data);

        return enemy;
    }

    private EnemyData PickRandom()
    {
        if (m_DataList == null || m_DataList.Count == 0) return null;
        if (m_DataList.Count == 1) return m_DataList[0];

        float totalWeight = 0f;
        foreach (var d in m_DataList)
            totalWeight += d.spawnWeight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var d in m_DataList)
        {
            cumulative += d.spawnWeight;
            if (roll <= cumulative)
                return d;
        }

        return m_DataList[^1];
    }
}
