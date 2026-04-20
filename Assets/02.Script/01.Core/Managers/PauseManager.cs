using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject pauseContainer;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;

    [SerializeField] private GameObject tipsContainer;
    [SerializeField] private GameObject[] tipsPanels;
    private int tipsPanelIdx;

    private void Start()
    {
        playerInputReader.PauseEvent += OnPause;
        playerInputReader.ResumeEvent += OnResume;
        pauseContainer.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playerInputReader != null)
        {
            playerInputReader.PauseEvent -= OnPause;
            playerInputReader.ResumeEvent -= OnResume;
        }
    }



    private void OnPause()
    {
        if (tipsContainer.activeSelf)
        {
            tipsContainer.SetActive(false);

            return;
        }
        else if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
            pausePanel.SetActive(true);
            return;
        }
        
        Time.timeScale = 0f;

        pauseContainer.SetActive(true);
        pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerInputReader.SetUIInput("Pause");
    }

    public void OnResume()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;

            pauseContainer.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerInputReader.SetUIInput("Player");
        }

    }

    public void GoBackTitle()
    {
        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadSceneByName("Title");
    }


    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void CloseTipsContainer()
    {
        if (tipsContainer.activeSelf)
        {
            
            foreach (GameObject panel in tipsPanels)
            {
                panel.SetActive(false);
            }
            tipsContainer.SetActive(false);
            pausePanel.SetActive(true);
            
        }
    }

    public void OpenTipsContainer()
    {
        Debug.Log("Call Open Panel");
        if (!tipsContainer.activeSelf)
        {
            pausePanel.SetActive(false);

            tipsPanelIdx = 0;
            tipsContainer.SetActive(true);
            tipsPanels[tipsPanelIdx].SetActive(true);
        }
    }

    public void ClickNextBtn()
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
    
    public void ClickPrevBtn()
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