using UnityEngine;
using UnityEngine.UI;

public class TitleAction : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject tipsContainer;
    [SerializeField] private GameObject[] tipsPanels;
    [SerializeField] public Button startBtn;
    [SerializeField] private Button exitBtn;

    private int tipsPanelIdx;

    void Start()
    {
        if (GameSceneManager.Instance != null)
        {
            startBtn.onClick.AddListener(() => GameSceneManager.Instance.LoadSceneByName("Game"));
            exitBtn.onClick.AddListener(() => GameSceneManager.Instance.ExitGame());
        }

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingPanel();
            CloseTipsContainer();
        }
    }

    public void CloseSettingPanel()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OpenSettingPanel()
    {
        Debug.Log("Call Open Panel");
        if (!settingPanel.activeSelf)
        {
            settingPanel.SetActive(true);
        }
    }

    public void CloseTipsContainer()
    {
        if (tipsContainer.activeSelf)
        {
            tipsContainer.SetActive(false);
            foreach(GameObject panel in tipsPanels)
            {
                panel.SetActive(false);
            }
        }
    }

    public void OpenTipsContainer()
    {
        Debug.Log("Call Open Panel");
        if (!tipsContainer.activeSelf)
        {
            tipsPanelIdx = 0;
            tipsContainer.SetActive(true);
            tipsPanels[tipsPanelIdx].SetActive(true);
        }
    }

    public void clickNextBtn()
    {

        tipsPanels[tipsPanelIdx].SetActive(false);
        if (tipsPanelIdx == tipsPanels.Length - 1)
        {
            tipsPanelIdx = 0;
            tipsPanels[tipsPanelIdx].SetActive(true);
            return;
        }
        tipsPanelIdx++;
        tipsPanels[tipsPanelIdx].SetActive(true);
    }
    
    public void clickPrevBtn()
    {
        
        tipsPanels[tipsPanelIdx].SetActive(false);
        if (tipsPanelIdx == 0)
        {
            tipsPanelIdx = tipsPanels.Length - 1;
            tipsPanels[tipsPanelIdx].SetActive(true);
            return;
        }
        tipsPanelIdx--;
        tipsPanels[tipsPanelIdx].SetActive(true);
    }
}
