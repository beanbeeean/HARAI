using NUnit.Framework.Interfaces;
using UnityEditor.Rendering;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerHPManager hpManager;
    [SerializeField] private FlashlightManager flashlightManager;
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
            if (slots[i].itemType ==  ItemType.None)
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
                        // 팝업 알림 넣어야할 곳
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
                        // 팝업 알림 넣어야할 곳
                        //Debug.Log("배터리가 이미 가득 찼습니다.");
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


        // 버린 자리에 똑같이 겹치게 버릴 수 있게 할지.. 고민
        //Collider2D hit = Physics2D.OverlapCircle(dropPos, 0.5f);
        //if (hit == null || !hit.CompareTag("Item"))
        //{
            
        //}

        GameObject droppedObj = Instantiate(slots[index].itemPrefab, dropPos, Quaternion.identity);

        ItemObject itemObj = droppedObj.GetComponent<ItemObject>();
        if (itemObj != null)
        {
            itemObj.itemData = slots[index];
        }

        Debug.Log($"{slots[index].itemName}을(를) 현재 위치에 버렸습니다.");
        slots[index] = new ItemData();
        inventoryUI.UpdateUI();

        Destroy(droppedObj, 30f);
    }


    //private void ShowFullMessage() {  }
}