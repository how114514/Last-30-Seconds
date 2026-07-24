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
    [SerializeField] private int m_VarietySize = 3;

    [Header("Spawn Point")]
    [SerializeField] private Transform m_SpawnPoint;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitUntil(() => GameManager.HasStarted);

        while (!GameManager.IsGameOver)
        {
            int count = RuntimeData.Instance != null ? RuntimeData.Instance.enemySpawnCount : 1;
            float interval = RuntimeData.Instance != null ? RuntimeData.Instance.enemySpawnInterval : 3f;

            for (int i = 0; i < count && !GameManager.IsGameOver; i++)
            {
                Spawn();
                if (count > 1)
                    yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(interval);
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

    /// <summary>Get the active spawn pool (sliding window of m_VarietySize).</summary>
    private List<EnemyData> GetActivePool()
    {
        int variety = RuntimeData.Instance != null ? RuntimeData.Instance.enemyVariety : 0;
        int start = Mathf.Clamp(variety, 0, m_DataList.Count - m_VarietySize);
        return m_DataList.GetRange(start, m_VarietySize);
    }

    private EnemyData PickRandom()
    {
        var pool = GetActivePool();
        if (pool == null || pool.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var d in pool)
            totalWeight += d.spawnWeight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var d in pool)
        {
            cumulative += d.spawnWeight;
            if (roll <= cumulative)
                return d;
        }

        return pool[^1];
    }
}
