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

    public List<Vector3> walkablePoints = new List<Vector3>();
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
                    Vector3 worldPos = map.CellToWorld(bounds.position + new Vector3Int(x, y)) + new Vector3(0.5f, 0.5f, 0);

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
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMove2D>().currentFloor = floorIndex;
        }
        else if (collision.CompareTag("Enemy"))
        {
            var fsm = collision.GetComponent<EnemyFSMController>();
            if (collision.TryGetComponent(out MainMonster mainMonster))
            {
                mainMonster.UpdateCurrentFloor(floorIndex);
            }
        }
    }
}