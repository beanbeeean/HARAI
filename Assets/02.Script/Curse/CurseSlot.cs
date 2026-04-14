using TMPro;
using UnityEngine;

public class CurseSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetSlotData(int number, string name, string description)
    {
        numberText.text = number.ToString();
        nameText.text = name;
        descriptionText.text = description;
    }
}
