using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private float navMeshCheckRadius = 0.2f;

    private List<Vector3> possibleTiles = new List<Vector3>();

    private void Start()
    {
        tilemap.CompressBounds();
    }

    void CalculatePossibleTiles()
    {
    }
}
