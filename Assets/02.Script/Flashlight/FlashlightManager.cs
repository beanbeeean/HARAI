using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightManager : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject flashlightObj;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private BatteryPopup batteryPopup;

    [Header("Setting")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color uvColor;
    public float maxPower = 100f;
    public float currentPower;
    [SerializeField] private float consumeRate = 1.0f;
    [SerializeField] private float rechargeRate = 0.5f;

    public bool isPowerOn = false;
    public bool isUVMode = false;

    public event Action<float, float> OnPowerChanged;

    private void Awake()
    {
        currentPower = maxPower;
    }

    private void OnEnable()
    {
        if(playerInputReader != null)
        {
            playerInputReader.FlashlightPowerStartedEvent += TogglePower;
            playerInputReader.FlashlightModeStartedEvent += ToggleMode;

        }
    }

    private void OnDisable()
    {
        if (playerInputReader != null)
        {
            playerInputReader.FlashlightPowerStartedEvent -= TogglePower;
            playerInputReader.FlashlightModeStartedEvent -= ToggleMode;

        }
    }

    private void Update()
    {
        HandlePower();
        OnPowerChanged?.Invoke(currentPower, maxPower);
    }

    void TogglePower()
    {
        isPowerOn = !isPowerOn;
        UpdateLightState();
    }

    void ToggleMode()
    {
        if (currentPower < consumeRate) return;

        isUVMode = !isUVMode;
        UpdateLightState();
    }

    void UpdateLightState()
    {
        flashlightObj.SetActive(isPowerOn);

        if (isPowerOn)
        {
            flashlight.color = isUVMode ? uvColor : normalColor;
        }
    }

    private void HandlePower()
    {
        if(isPowerOn && isUVMode)
        {
            currentPower -= consumeRate * Time.deltaTime;

            if(currentPower <= 0)
            {
                currentPower = 0;
                isUVMode = false;
                UpdateLightState();
            }
        }
        else
        {
            if(currentPower < maxPower)
            {
                
                currentPower += rechargeRate * Time.deltaTime;
                currentPower = Mathf.Min(currentPower, maxPower);
            }
        }
    }

    public void AddPower(float amount)
    {
        currentPower = Mathf.Clamp(currentPower + amount, 0, maxPower);
        batteryPopup.Show((int) amount);
    }

    
}
