using UnityEngine;
using System.Collections.Generic;

public class CurseUIController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private RectTransform contentTransform;
    
    public void RefreshUI()
    {
        foreach (RectTransform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        List<CurseData> activeCurses = CurseManager.instance.GetActiveCurseList();

        for (int i = 0; i < activeCurses.Count; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, contentTransform);
            CurseSlot curseSlot = newSlot.GetComponent<CurseSlot>();

            if (curseSlot != null)
            {
                curseSlot.SetSlotData(i + 1, activeCurses[i].curseName, activeCurses[i].curseDescription);
            }
        } 
    }
}
