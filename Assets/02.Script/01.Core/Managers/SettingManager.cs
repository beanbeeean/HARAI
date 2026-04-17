using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{

    [Header("Audio Settings UI")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmValueText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;



    void Start()
    {
        
        bgmSlider.onValueChanged.AddListener(UpdateBGMVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);

        bgmSlider.value = SoundManager.Instance.GetCustomBGMVolume();
        sfxSlider.value = SoundManager.Instance.GetCustomSFXVolume();

        UpdateVolumeText(bgmSlider.value, bgmValueText);
        UpdateVolumeText(sfxSlider.value, sfxValueText);
    }

    private void UpdateBGMVolume(float value)
    {
        UpdateVolumeText(value, bgmValueText);

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(value);
    }

    private void UpdateSFXVolume(float value)
    {
        UpdateVolumeText(value, sfxValueText);

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }

    private void UpdateVolumeText(float value, TextMeshProUGUI text)
    {
        int intValue = (int)(value * 100);
        text.text = intValue.ToString();
    }

}
