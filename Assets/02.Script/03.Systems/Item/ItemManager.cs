using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

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

    public ItemData GetItemCopy(int itemID)
    {
        ItemData origin = itemDatabase.Find(item => item.itemID ==  itemID);
        if(origin != null)
        {
            return new ItemData(origin);
        }

        return null;
    }

    public ItemData GetRandomItemCopy()
    {
        if(itemDatabase.Count == 0)
        {
            return null;
        }

        int totalWeight = 0;
        foreach (ItemData item in itemDatabase) {
            totalWeight += item.spawnWeight;
        }

        int randomValue = Random.Range(0, totalWeight);

        int currentWeightSum = 0;
        foreach (ItemData item in itemDatabase)
        {
            currentWeightSum += item.spawnWeight;

            if(randomValue < currentWeightSum)
            {
                return new ItemData(item);
            }
        }

        return new ItemData(itemDatabase[0]);
    }
}
