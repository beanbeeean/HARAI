using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject enemyPrefab;
    public Transform playerTransform;

    [Header("Respawn Logic")]
    public float minSpawnDistance = 8f;
    public float respawnTime = 15f;

    private List<MapArea> allAreas = new List<MapArea>();

    private void Start()
    {
        allAreas.AddRange(FindObjectsByType<MapArea>(FindObjectsSortMode.None));

        foreach (var area in allAreas)
        {
            area.InitializeArea();
            StartCoroutine(InitialSpawn(area));
        }
    }

    private IEnumerator InitialSpawn(MapArea area)
    {
        for (int i = 0; i < area.maxEnemyCount; i++)
        {
            SpawnMonsterInArea(area);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnMonsterInArea(MapArea area)
{
    if (area.walkablePoints.Count == 0) return;

    Vector3 spawnPos = GetValidSpawnPoint(area);
    GameObject monster = EnemyPool.Instance.Get(spawnPos);
    
    var controller = monster.GetComponent<EnemyFSMController>();
    if (controller != null)
    {
        controller.target = playerTransform;
    }

    EnemyDeathNotifier notifier = monster.GetComponent<EnemyDeathNotifier>();
    if (notifier == null)
    {
        notifier = monster.AddComponent<EnemyDeathNotifier>();
    }
    notifier.Init(this, area); 
    
    Debug.Log($"{area.name} 구역에 몬스터 재생성 요청 완료");
}

    private Vector3 GetValidSpawnPoint(MapArea area)
    {
        List<Vector3> points = area.walkablePoints;

        for (int i = 0; i < 30; i++)
        {
            Vector3 candidate = points[Random.Range(0, points.Count)];
            float distanceToPlayer = Vector2.Distance(candidate, playerTransform.position);

            if (distanceToPlayer > minSpawnDistance)
            {
                return candidate;
            }
        }

        return points[Random.Range(0, points.Count)];
    }

    public void OnMonsterDied(MapArea area)
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(RespawnRoutine(area));
        }
    }

    private IEnumerator RespawnRoutine(MapArea area)
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnMonsterInArea(area);
    }
}

