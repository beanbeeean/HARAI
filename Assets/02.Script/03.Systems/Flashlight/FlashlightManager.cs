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

    [SerializeField] private PlayerHPManager playerHPManager;

    [Header("Setting")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color uvColor;
    public float maxPower = 100f;
    public float baseMaxPower = 100f;
    public float currentPower;
    [SerializeField] private float consumeRate = 5.0f;
    [SerializeField] private float rechargeRate = 2.0f;
    [SerializeField] private float baseRechargeRate = 2.0f;

    public bool isPowerOn = false;
    public bool isUVMode = false;

    public event Action<float, float> OnPowerChanged;

    private void Awake()
    {
        currentPower = maxPower;
    }

    private void OnEnable()
    {
        if (playerInputReader != null)
        {
            playerInputReader.FlashlightPowerStartedEvent += TogglePower;
            playerInputReader.FlashlightModeStartedEvent += ToggleMode;

        }
        
        if(playerHPManager != null)
        {
            playerHPManager.OnDied += TurnOff;
        }
    }

    private void OnDisable()
    {
        if (playerInputReader != null)
        {
            playerInputReader.FlashlightPowerStartedEvent -= TogglePower;
            playerInputReader.FlashlightModeStartedEvent -= ToggleMode;

        }

        if(playerHPManager != null)
        {
            playerHPManager.OnDied -= TurnOff;
        }
    }

    private void Update()
    {
        HandlePower();
        OnPowerChanged?.Invoke(currentPower, maxPower);
    }

    public void TurnOff()
    {
        Debug.Log("TurnOff");
        isPowerOn = false;
        SpriteRenderer flashlight = GetComponent<SpriteRenderer>();
        flashlight.enabled = false;
        flashlightObj.SetActive(false);
    }

    void TogglePower()
    {
        isPowerOn = !isPowerOn;
        SoundManager.Instance.PlaySFX("Flashlight");
        UpdateLightState();
    }

    void ToggleMode()
    {
        if (currentPower < consumeRate) return;
        isUVMode = !isUVMode;
        SoundManager.Instance.PlaySFX("Flashlight");
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

    public void UpdateRechargeTime(float totalPenalty)
    {
        rechargeRate = Mathf.Max(baseRechargeRate - totalPenalty, 1.1f);
    }

    public void UpdateMaxPower(float totalPenalty)
    {
        maxPower = Mathf.Max(baseMaxPower - totalPenalty, 70f);
        OnPowerChanged?.Invoke(currentPower, maxPower);
    }
}
