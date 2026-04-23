using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [Header("Spawn Setting")]
    [SerializeField] private int spawnLimit = 10;
    [SerializeField] private float respawnDelay = 10f;
    [SerializeField] private float itemLifeTime = 15f;

    [SerializeField] private List<SpawnPointState> pointStates = new List<SpawnPointState>();


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        foreach (Transform child in transform)
        {
            pointStates.Add(new SpawnPointState(child));
        }



        InitialSpawn();
    }

    private void Update()
    {
        UpdateStates();
        TrySpawnItem();
    }

    void UpdateStates()
    {
        foreach (SpawnPointState state in pointStates)
        {
            if (state.currentItem == null && state.wasSpawning)
            {
                state.wasSpawning = false;
                state.isCooling = true;
                state.respawnTimer = respawnDelay;
            }

            if (state.isCooling)
            {
                state.respawnTimer -= Time.deltaTime;
                if (state.respawnTimer <= 0)
                {
                    state.isCooling = false;
                }
            }
        }
    }

    void TrySpawnItem()
    {
        int activeCount = pointStates.FindAll(state => state.currentItem != null).Count;

        if(activeCount < spawnLimit)
        {
            List<SpawnPointState> emptyPoints = pointStates.FindAll(state =>!state.wasSpawning && !state.isCooling && state.currentItem == null);

            if (emptyPoints.Count > 0)
            {
                SpawnItemAt(emptyPoints[Random.Range(0, emptyPoints.Count)]);
            }
        }
    }
    void InitialSpawn()
    {

        List<SpawnPointState> available = new List<SpawnPointState>(pointStates);
        int count = Mathf.Min(spawnLimit, available.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, available.Count);
            SpawnItemAt(available[randomIndex]);
            available.RemoveAt(randomIndex);
        }
    }

    void SpawnItemAt(SpawnPointState state)
    {
        ItemData data = ItemManager.Instance.GetRandomItemCopy();
        if (data == null) return;

        GameObject newItem = Instantiate(data.itemPrefab, state.point.position, Quaternion.identity);

        ItemObject itemObj = newItem.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.itemData = data;
        }

        Destroy(newItem, itemLifeTime);
        state.currentItem = newItem;
        state.wasSpawning = true;
    }

    
}