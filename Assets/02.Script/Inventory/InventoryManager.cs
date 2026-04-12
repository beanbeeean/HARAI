using NUnit.Framework.Interfaces;
using UnityEditor.Rendering;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerHPManager hpManager;
    public ItemData[] slots = new ItemData[3];
    public float dropDistance = 1.0f;
    public InventoryUI inventoryUI;


    private void Awake()
    {
        if(inputReader == null)
        {
            inputReader = GetComponent<PlayerInputReader>();

        }
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnInventoryUsed += UseItem;
            inputReader.OnInventoryDropped += DropItem;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnInventoryUsed -= UseItem;
            inputReader.OnInventoryDropped -= DropItem;
        }
    }

    public bool AddItem(ItemData newItem)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = newItem;
                if (inventoryUI != null)
                {
                    inventoryUI.UpdateUI();
                }
                return true;
            }
        }
        //ShowFullMessage();
        inventoryUI.UpdateUI();
        return false;
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= slots.Length || slots[index] == null) return;

        ItemData item = slots[index];

        bool useSuccess = ApplyItemEffect(item);

        if (useSuccess)
        {
            slots[index] = null;
            inventoryUI?.UpdateUI();
        }
    }

    private bool ApplyItemEffect(ItemData data)
    {
        if (data == null) return false;

        switch (data.itemType)
        {
            case ItemType.Health: 
                if (hpManager != null)
                {
                    if (hpManager.CurrentHealth >= hpManager.MaxHealth)
                    {
                        Debug.Log("체력이 이미 가득 찼습니다!");
                        return false;
                    }

                    hpManager.Heal((int)data.value);

                    return true;
                }
                return false;

            case ItemType.Battery:
                return true;

            case ItemType.CurseRemover:
                return true;

            default:
                return false;
        }
    }

    private void DropItem(int index)
    {
        if (slots[index] == null) return;

        Vector3 dropPos = transform.position;

        GameObject droppedObj = Instantiate(slots[index].itemPrefab, dropPos, Quaternion.identity);

        ItemObject itemObj = droppedObj.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.itemData = slots[index];
        }

        Debug.Log($"{slots[index].itemName}을(를) 현재 위치에 버렸습니다.");
        slots[index] = null;
        inventoryUI.UpdateUI();
    }


    //private void ShowFullMessage() {  }
}