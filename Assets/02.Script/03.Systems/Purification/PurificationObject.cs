using UnityEngine;
using UnityEngine.UI;

public class PurificationObject : InteractableBase
{
    [SerializeField] PlayerInputReader playerInputReader;

    [Header("Purify Settings")]
    [SerializeField] private string ObjectName;
    [SerializeField] private float totalPurifyTime = 5.0f;
    [SerializeField] private float currentProgress = 0f;

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float unexposedAlpha = 0.05f;

    [Header("UI Ref")]
    [SerializeField] private GameObject gaugeCanvas;
    [SerializeField] private Slider progressSlider;

    [Header("Status")]
    [SerializeField] private bool isColliding = false;
    [SerializeField] private FlashlightManager flashlightManager;
    

    [Header("Effect Settings")]
    [SerializeField] private GameObject purificationEffectPrefab;
    [SerializeField] private float effectYOffset = 1.0f;
    
    private readonly int lightLayerMask = 1 << 13;

    [SerializeField] private FlashlightDetector flashlightDetector;

    
    protected override void Awake()
    {
        base.Awake();

        SetAlpha(unexposedAlpha);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = totalPurifyTime;
            progressSlider.value = 0;
        }

        if (gaugeCanvas != null) gaugeCanvas.SetActive(false);

        if (playerInputReader == null)
        {
            playerInputReader = FindFirstObjectByType<PlayerInputReader>();
        }
    }

    protected override void Update()
    {
        base.Update();

        bool isExposed = isColliding && flashlightManager != null &&
                        flashlightManager.isPowerOn && flashlightManager.isUVMode;

        SetAlpha(isExposed ? 1.0f : unexposedAlpha);

        if (isExposed)
            {
                // Debug.Log("flashlightDetector.IsInRange : " + flashlightDetector.IsInRange);
            if (flashlightDetector.IsInRange)
            {
                    // Debug.Log("통과");
                ShowUI();
                if (gaugeCanvas != null) gaugeCanvas.SetActive(true);

                if (playerInputReader.InteractPressed)
                {
                    currentProgress += Time.deltaTime;
                    if (progressSlider != null) progressSlider.value = currentProgress;
                    if (currentProgress >= totalPurifyTime) CompletePurification();
                }
            }
            else
            {
                HideUI();
                if (gaugeCanvas != null) gaugeCanvas.SetActive(false);
            }
        }
        else
        {
            HideUI();
            if (gaugeCanvas != null) gaugeCanvas.SetActive(false);
        }
    }

    public override void Interact() { }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    private void UpdateVisuals(bool exposed)
    {
        SetAlpha(exposed ? 1.0f : unexposedAlpha);

        if (exposed) ShowUI(); else HideUI();
    }

 

    private void CompletePurification()
    {
        Debug.Log("정화 완료!");
        SoundManager.Instance.PlaySFX(SoundType.Purificate);
        if (purificationEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, effectYOffset, 0);

            GameObject effect = Instantiate(purificationEffectPrefab, spawnPosition, Quaternion.identity);

            Destroy(effect, 0.8f);
        }
        else
        {
            Debug.LogWarning("이펙트 프리팹 할당 X");
        }

        if (PurificationManager.Instance != null)
        {
            PurificationManager.Instance.OnObjectPurified();
        }
        else
        {
            Debug.LogWarning("PurificationManager 없음");
        }

       

        if(CurseManager.instance != null  && PurificationManager.Instance.CurrentPurified < PurificationManager.Instance.TotalTargets - 1)
        {
            
            CurseManager.instance.AddRandomCurse();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & lightLayerMask) != 0)
        {
            if (flashlightManager != null)
            {
                isColliding = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & lightLayerMask) != 0)
        {
            isColliding = false;
        }
    }
}