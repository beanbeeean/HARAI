using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerHPManager hpManager;
    [SerializeField] private FlashlightManager flashlightManager;
    public ItemData[] slots = new ItemData[3];
    public float dropDistance = 1.0f;
    public InventoryUI inventoryUI;
    private Queue<ItemData> pickItems = new Queue<ItemData>();

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
        pickItems.Enqueue(newItem);
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemType == ItemType.None)
            {
                slots[i] = pickItems.Dequeue();
                if (inventoryUI != null)
                {
                    inventoryUI.UpdateUI();
                }
                SoundManager.Instance.PlaySFX(SoundType.PickDropItem);
                return true;
            }
        }
        pickItems.Clear();
        AlertManager.Instance.ShowAlert(AlertKey.CannotPickUp);
        inventoryUI.UpdateUI();
        return false;
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= slots.Length || slots[index].itemType ==  ItemType.None) return;

        ItemData item = slots[index];

        bool useSuccess = ApplyItemEffect(item);

        if (useSuccess)
        {
            slots[index] = new ItemData();
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
                        AlertManager.Instance.ShowAlert(AlertKey.FullHealth);
                        return false;
                    }

                    hpManager.Heal((int)data.value);

                    return true;
                }
                return false;

            case ItemType.Battery:
                if(flashlightManager != null)
                {
                    if(flashlightManager.currentPower >= flashlightManager.maxPower)
                    {
                       
                        AlertManager.Instance.ShowAlert(AlertKey.FullBattery);
                        return false;
                    }
                    flashlightManager.AddPower(data.value);
                    return true;
                }
                return false;

            case ItemType.CurseRemover:
                if(CurseManager.instance != null)
                {
                    if(CurseManager.instance.GetActiveCurseList().Count <= 0)
                    {
                        AlertManager.Instance.ShowAlert(AlertKey.NoCurse);
                        return false;
                    }
                    else if (CurseManager.instance.IsCleanseOnCooldown)
                    {
                        AlertManager.Instance.ShowAlert(AlertKey.CannotCleanse, (int)CurseManager.instance.currentCleanseCDT);
                        return false;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySFX(SoundType.UseBrokenGlass);
                        CurseManager.instance.RemoveLastCurse();
                        return true;
                    }
                }
                return false;

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
        SoundManager.Instance.PlaySFX(SoundType.PickDropItem);
        Debug.Log($"{slots[index].itemName}을(를) 현재 위치에 버렸습니다.");
        slots[index] = new ItemData();
        inventoryUI.UpdateUI();
        droppedObj.GetComponent<ItemObject>().DestoryItem();
    }


}