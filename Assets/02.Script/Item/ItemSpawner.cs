using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public List<ItemData> itemPool;
    public int maxItemsInMap = 10;
    public float respawnCheckTime = 5f;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> currentItems = new List<GameObject>();

    void Start()
    {
        spawnPoints.Clear();

        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} 아래에 자식 스폰 포인트가 하나도 없습니다!");
        }
        else
        {
            Debug.Log($"{gameObject.name}: {spawnPoints.Count}개의 스폰 포인트를 로드했습니다.");
        }

        InitialSpawn();
        InvokeRepeating(nameof(CheckAndRespawn), respawnCheckTime, respawnCheckTime);
    }

    void InitialSpawn()
    {
        while (currentItems.Count < maxItemsInMap && spawnPoints.Count > 0)
        {
            SpawnRandomItem();
        }
    }

    void CheckAndRespawn()
    {
        currentItems.RemoveAll(item => item == null);

        if (currentItems.Count < maxItemsInMap)
        {
            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        List<Transform> validPoints = GetEmptySpawnPoints();
        if (validPoints.Count == 0) return;

        Transform selectedPoint = validPoints[Random.Range(0, validPoints.Count)];
        ItemData selectedData = itemPool[Random.Range(0, itemPool.Count)];

        GameObject newItem = Instantiate(selectedData.itemPrefab, selectedPoint.position, Quaternion.identity);
        newItem.GetComponent<ItemObject>().itemData = selectedData;

        currentItems.Add(newItem);
    }

    List<Transform> GetEmptySpawnPoints()
    {
        List<Transform> emptyPoints = new List<Transform>();
        foreach (var sp in spawnPoints)
        {
            Collider2D hit = Physics2D.OverlapCircle(sp.position, 0.5f);
            if (hit == null || !hit.CompareTag("Item"))
            {
                emptyPoints.Add(sp);
            }
        }
        return emptyPoints;
    }
}