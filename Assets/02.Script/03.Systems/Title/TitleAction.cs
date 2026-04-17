using UnityEngine;
using UnityEngine.UI;

public class TitleAction : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] public Button startBtn;
    [SerializeField] private Button exitBtn;

    void Start()
    {
        if(GameSceneManager.Instance != null)
        {
            startBtn.onClick.AddListener(() => GameSceneManager.Instance.LoadSceneByName("Game"));
            exitBtn.onClick.AddListener(() => GameSceneManager.Instance.ExitGame());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OpenPanel()
    {
        Debug.Log("Call Open Panel");
        if (!settingPanel.activeSelf)
        {
            settingPanel.SetActive(true);
        }
    }
}
