using UnityEngine;

/// <summary>
/// Spawns a single boss at game start. No intervals, no repeat.
/// </summary>
public class BossSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject m_Prefab;

    [Header("Spawn Point")]
    [SerializeField] private Transform m_SpawnPoint;

    private EnemyData m_Data;
    private bool m_Spawned;

    public void SetBossData(EnemyData data)
    {
        m_Data = data;
    }

    private void Update()
    {
        if (m_Spawned) return;
        if (!GameManager.HasStarted) return;

        m_Spawned = true;
        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (m_Prefab == null || m_Data == null) return;

        var pos = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
        var boss = Instantiate(m_Prefab, pos, Quaternion.identity);

        boss.transform.localScale = Vector3.one * 3f;

        var stats = boss.GetComponent<EnemyStats>();
        if (stats != null) stats.ApplyData(m_Data);
    }
}
