using UnityEngine;
using UnityEngine.UI;

public class TitleAction : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject tipsContainer;
    [SerializeField] private GameObject confirmContainer;
    
    [SerializeField] private GameObject[] tipsPanels;
    [SerializeField] public Button startBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] Animator backgroundAnimator;
    [SerializeField] private float blinkerTimer = 10.0f;

    private int tipsPanelIdx;

    void Start()
    {
        InvokeRepeating("BlinkerBackground", blinkerTimer, blinkerTimer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingPanel();
            CloseTipsContainer();
            CloseConfirmContainer();
        }
    }

    public void BlinkerBackground()
    {
        int hashTrigger = Animator.StringToHash("BlinkerTrigger");
        backgroundAnimator.SetTrigger(hashTrigger);
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

    public void OpenConfirmContainer()
    {
        Debug.Log("Call Open Panel");
        if (!confirmContainer.activeSelf)
        {
            confirmContainer.SetActive(true);
        }
    }

    public void CloseConfirmContainer()
    {
        Debug.Log("Call Open Panel");
        if (confirmContainer.activeSelf)
        {
            confirmContainer.SetActive(false);
        }
    }

    public void ClickConfirmBtnToStory()
    {
        GameSceneManager.Instance.LoadSceneByName("Story");
    }

    public void ClickConfirmBtnToGame()
    {
        GameSceneManager.Instance.LoadSceneByName("Game");
    }

    public void CloseTipsContainer()
    {
        if (tipsContainer.activeSelf)
        {
            tipsContainer.SetActive(false);
            foreach (GameObject panel in tipsPanels)
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
    
    public void ClickExitBtn()
    {
        GameSceneManager.Instance.ExitGame();
    }
}
