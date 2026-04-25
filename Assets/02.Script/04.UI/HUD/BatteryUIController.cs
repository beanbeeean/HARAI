using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUIController : MonoBehaviour
{
    [SerializeField] FlashlightManager flashlightManager;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TextMeshProUGUI powerText;

    private void OnEnable()
    {
        flashlightManager.OnPowerChanged += UpdateHPUI; 
    }

    private void OnDisable()
    {
        flashlightManager.OnPowerChanged -= UpdateHPUI;
    }


    void UpdateHPUI(float currentPower, float maxPower)
    {
        float ratio = currentPower / maxPower;
        powerSlider.value = ratio;
        powerText.text = $"PW: {(int)currentPower}/{(int)maxPower}";
    }

}
