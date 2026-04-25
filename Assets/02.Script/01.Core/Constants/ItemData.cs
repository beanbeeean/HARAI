using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public float value;
    public GameObject itemPrefab;
    public int spawnWeight;

    public ItemData(ItemData data)
    {
        this.itemID = data.itemID;
        this.itemName = data.itemName;
        this.itemType = data.itemType;
        this.itemIcon = data.itemIcon;
        this.value = data.value;
        this.itemPrefab = data.itemPrefab;
        this.spawnWeight = data.spawnWeight;
    }

    public ItemData() {
        this.itemID = 0;
        this.itemName = "";
        this.itemType = ItemType.None;
        this.itemIcon = null;
        this.value = 0f;
        this.itemPrefab = null;
        this.spawnWeight = 0;
    }
}