using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject enemyPrefab;        // 소환할 몬스터 프리팹
    public Transform playerTransform;      // 플레이어 위치 추적용

    [Header("Respawn Logic")]
    public float minSpawnDistance = 8f;   // 플레이어로부터 최소 이 거리만큼 떨어져야 함
    public float respawnTime = 15f;       // 몬스터 사망 후 리스폰 대기 시간

    private List<MapArea> allAreas = new List<MapArea>();

    private void Start()
    {
        // 1. 씬에 배치된 모든 MapArea를 찾아서 초기화
        allAreas.AddRange(FindObjectsByType<MapArea>(FindObjectsSortMode.None));

        foreach (var area in allAreas)
        {
            area.InitializeArea();
            StartCoroutine(InitialSpawn(area));
        }
    }

    // 게임 시작 시 구역별 설정된 최대 마리수만큼 소환
    private IEnumerator InitialSpawn(MapArea area)
    {
        for (int i = 0; i < area.maxEnemyCount; i++)
        {
            SpawnMonsterInArea(area);
            yield return new WaitForSeconds(0.1f); // 동시 생성 부하 방지
        }
    }

    private void SpawnMonsterInArea(MapArea area)
    {
        if (area.walkablePoints.Count == 0) return;

        Vector3 spawnPos = GetValidSpawnPoint(area);
        GameObject monster = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 몬스터에게 필요한 타겟 정보 주입
        var controller = monster.GetComponent<EnemyFSMController>();
        if (controller != null)
        {
            controller.target = playerTransform;
        }

        // [중요] 몬스터에게 '사망 감지기'를 달아줍니다.
        EnemyDeathNotifier notifier = monster.AddComponent<EnemyDeathNotifier>();
        notifier.Init(this, area);
    }

    // 플레이어와 너무 가깝지 않은 랜덤 좌표 찾기
    private Vector3 GetValidSpawnPoint(MapArea area)
    {
        List<Vector3> points = area.walkablePoints;

        // 유효한 지점을 찾기 위해 최대 30번 시도
        for (int i = 0; i < 30; i++)
        {
            Vector3 candidate = points[Random.Range(0, points.Count)];
            float distanceToPlayer = Vector2.Distance(candidate, playerTransform.position);

            // 플레이어와 최소 거리 이상일 때만 선택
            if (distanceToPlayer > minSpawnDistance)
            {
                return candidate;
            }
        }

        // 만약 모든 지점이 플레이어와 가깝다면 (좁은 방 등), 그냥 가장 먼 곳이라도 선택
        return points[Random.Range(0, points.Count)];
    }

    // 몬스터가 죽었을 때 호출될 함수
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

// 몬스터가 파괴(Destroy)될 때를 감지하여 Spawner에게 알리는 보조 클래스
