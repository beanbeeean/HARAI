using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryManager invManager;
    public Image[] slotIcons;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        //Debug.Log("UpdateUI 호출됨!");
        for (int i = 0; i < invManager.slots.Length; i++)
        {
            if (invManager.slots[i] != null)
            {
                Debug.Log($"{i}번 슬롯 아이템 있음: {invManager.slots[i].itemName}");
                slotIcons[i].sprite = invManager.slots[i].itemIcon;
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