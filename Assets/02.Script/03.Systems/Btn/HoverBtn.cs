using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Color originColor;
    [SerializeField] private Color hoverColor = new Color(113f, 52f, 125f);
    void Awake()
    {
        if(text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            originColor = text.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originColor;
    }
}
