using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class MapArea : MonoBehaviour
{
    public string areaName;
    public int maxEnemyCount = 3;
    [SerializeField] private int floorIndex;

    [Header("Layer Settings")]
    [SerializeField] private string floorLayerName = "Floor"; 

    [HideInInspector] public List<Vector3> walkablePoints = new List<Vector3>();
    private float navMeshCheckRadius = 0.1f;

    public void InitializeArea()
    {
        walkablePoints.Clear();

        int targetLayer = LayerMask.NameToLayer(floorLayerName);
        if (targetLayer == -1)
        {
            Debug.LogError($"{gameObject.name}: {floorLayerName} 레이어 찾지못함");
            return;
        }

        Tilemap[] childMaps = GetComponentsInChildren<Tilemap>();

        foreach (var map in childMaps)
        {
            if (map.gameObject.layer == targetLayer)
            {
                ExtractWalkablePoints(map);
            }
        }

        Debug.Log($"{areaName} 구역: {walkablePoints.Count}개 포인트 수집 완료");
    }

    private void ExtractWalkablePoints(Tilemap map)
    {
        map.CompressBounds();
        BoundsInt bounds = map.cellBounds;
        TileBase[] tiles = map.GetTilesBlock(bounds);

        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = tiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    // 타일 중심 좌표 계산
                    Vector3 worldPos = map.CellToWorld(bounds.position + new Vector3Int(x, y)) + new Vector3(0.5f, 0.5f, 0);

                    // NavMesh 위(하늘색 바닥)인지 확인
                    if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas))
                    {
                        walkablePoints.Add(hit.position);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어나 몬스터가 들어오면 층 정보를 업데이트
        if (collision.CompareTag("Player"))
        {
            // 플레이어 스크립트에 floorIndex 전달 (기능 추가 필요)
            collision.GetComponent<PlayerMove2D>().currentFloor = floorIndex;
        }
        else if (collision.CompareTag("Enemy"))
        {
            // 메인 몬스터나 일반 몬스터에게 전달
            var fsm = collision.GetComponent<EnemyFSMController>();
            // 여기서 메인 몬스터인지 체크해서 층 정보 업데이트
            if (collision.TryGetComponent(out MainMonster mainMonster))
            {
                mainMonster.UpdateCurrentFloor(floorIndex);
            }
        }
    }
}