using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Image[] slotIcons;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        //Debug.Log("UpdateUI 호출됨!");
        for (int i = 0; i < inventoryManager.slots.Length; i++)
        {
            if (inventoryManager.slots[i].itemType != ItemType.None)
            {
                Debug.Log($"{i}번 슬롯 아이템 있음: {inventoryManager.slots[i].itemName}");
                slotIcons[i].sprite = inventoryManager.slots[i].itemIcon;
                slotIcons[i].enabled = true;

                slotIcons[i].color = Color.white;
            }
            else
            {
                slotIcons[i].enabled = false;
            }
        }
    }
}