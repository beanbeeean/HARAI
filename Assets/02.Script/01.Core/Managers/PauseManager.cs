using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject pauseContainer;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;


    // [Header("Audio Settings UI")]
    // [SerializeField] private Slider bgmSlider;
    // [SerializeField] private TextMeshProUGUI bgmValueText;
    // [SerializeField] private Slider sfxSlider;
    // [SerializeField] private TextMeshProUGUI sfxValueText;

    private void Start()
    {
        playerInputReader.PauseEvent += OnPause;
        playerInputReader.ResumeEvent += OnResume;

        // bgmSlider.onValueChanged.AddListener(UpdateBGMVolume);
        // sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);

        // float defaultStartValue = 0.5f;
        // bgmSlider.value = defaultStartValue;
        // sfxSlider.value = defaultStartValue;

        // UpdateVolumeText(bgmSlider.value, bgmValueText);
        // UpdateVolumeText(sfxSlider.value, sfxValueText);

        // Debug.Log($"bgmSlider.value: {bgmSlider.value}");
        // Debug.Log($"sfxSlider.value: {sfxSlider.value}");
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

    // private void UpdateBGMVolume(float value)
    // {
    //     UpdateVolumeText(value, bgmValueText);

    //     if (SoundManager.Instance != null)
    //         SoundManager.Instance.SetBGMVolume(value);
    // }

    // private void UpdateSFXVolume(float value)
    // {
    //     UpdateVolumeText(value, sfxValueText);

    //     if (SoundManager.Instance != null)
    //         SoundManager.Instance.SetSFXVolume(value);
    // }

    // private void UpdateVolumeText(float value, TextMeshProUGUI text)
    // {
    //     text.text = Mathf.RoundToInt(value * 100).ToString();
    // }

    private void OnPause()
    {
        Time.timeScale = 0f;

        pauseContainer.SetActive(true);
        pausePanel.SetActive(true);
        settingPanel.SetActive(false);

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

    //public void GoToTitle()
    //{
    //    Time.timeScale = 1f;
    //}
}