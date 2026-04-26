using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightManager : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject flashlightObj;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private GameObject subLightObj;

    [SerializeField] private Light2D keyBoardFlashlight;
    [SerializeField] private GameObject keyboardSubLightObj;

    [SerializeField] private Light2D mouseFlashlight;
    [SerializeField] private GameObject mouseSubLightObj;

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

    [Header("Rotation Settings")]
    [SerializeField] private Transform flashlightPivot;
    [SerializeField] private float viewAngleLimit = 180f;
    [SerializeField] private PlayerMove2D playerMove2D; 
    [SerializeField] private float angleOffset = -225f;

    public bool isPowerOn = false;
    public bool isUVMode = false;

    public event Action<float, float> OnPowerChanged;

    private void Awake()
    {
        currentPower = maxPower;
        flashlight = mouseFlashlight;
        flashlightObj = mouseFlashlight.gameObject;
        subLightObj = mouseSubLightObj;
        flashlightPivot = mouseFlashlight.transform;
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
        RotateFlashlight();
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
        subLightObj.SetActive(false);
    }

    void TogglePower()
    {
        isPowerOn = !isPowerOn;
        SoundManager.Instance.PlaySFX(SoundType.Flashlight);
        UpdateLightState();
    }

    void ToggleMode()
    {
        if (currentPower < consumeRate) return;
        isUVMode = !isUVMode;
        SoundManager.Instance.PlaySFX(SoundType.Flashlight);
        UpdateLightState();
    }

    void UpdateLightState()
    {
        flashlightObj.SetActive(isPowerOn);
        subLightObj.SetActive(isPowerOn);

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

    
    private void RotateFlashlight()
{
    if (flashlightPivot == null) return;

    // 1. 마우스 위치를 월드 좌표로 변환
    Vector3 mouseInput = Input.mousePosition;
    mouseInput.z = 10f; 
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseInput);
    mousePos.z = 0f;

    // 2. 피벗에서 마우스까지의 방향 및 절대 각도 계산
    Vector2 dir = (mousePos - flashlightPivot.position).normalized;
    float targetMouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    // 3. 플레이어 각도 계산 없이, 마우스 각도에 보정값(Offset)만 더함
    // angleOffset과 135f는 현재 사용 중인 스프라이트의 기본 기울기에 맞춰 조절하세요.
    float finalAngle = targetMouseAngle + (angleOffset + 135f);

    // 4. 회전 적용
    flashlightPivot.rotation = Quaternion.Euler(0, 0, finalAngle);
}
}
