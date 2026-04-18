using UnityEngine;

public class TooltipsManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject tooltipsPanel;


    void OnEnable()
    {
        playerInputReader.TooltipsEvent += TooltipsToggle;
    }

    void OnDisable()
    {
        playerInputReader.TooltipsEvent -= TooltipsToggle;
    }

    void TooltipsToggle()
    {
        if (tooltipsPanel.activeSelf)
        {
            tooltipsPanel.SetActive(false);
        }
        else
        {
            tooltipsPanel.SetActive(true);
        }
    }
}
