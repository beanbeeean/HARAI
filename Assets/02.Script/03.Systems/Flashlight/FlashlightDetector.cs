using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightDetector : MonoBehaviour
{
    [SerializeField] Image detectionPopup;
    [SerializeField] private Sprite searchSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Color farColor;
    [SerializeField] private Color nearColor;

    [SerializeField] private Color activeFarColor;
    [SerializeField] private Color activeNearColor;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;


    [Header("Pulse Settings")]
    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float pulseSpeed = 5f;

    private Coroutine pulseCoroutine;
    private Vector3 originalScale;

    public bool IsInRange { get; private set; }


    void Awake()
    {
        if (detectionPopup != null) originalScale = detectionPopup.rectTransform.localScale;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            detectionPopup.enabled = true;
            if (pulseCoroutine == null)
            {
              pulseCoroutine = StartCoroutine(PulsePopup());  
            } 
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            float distance = Vector2.Distance(transform.position, collision.transform.position);
            float half = maxDistance * 0.5f;

            if (distance > half)
            {
                IsInRange = false;
                detectionPopup.sprite = searchSprite;
                float percent = (maxDistance - distance) / (maxDistance - half);
                percent = Mathf.Clamp01(percent);
                detectionPopup.color = Color.Lerp(farColor, nearColor, percent);
            }
            else
            {
                IsInRange = true;
                detectionPopup.sprite = activeSprite;
                float activePercent = (half - distance) / (half - minDistance);
                activePercent = Mathf.Clamp01(activePercent);
                detectionPopup.color = Color.Lerp(activeFarColor, activeNearColor, activePercent);
            }
        }
    }

    IEnumerator PulsePopup()
    {
        RectTransform rectTransform = detectionPopup.rectTransform;
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * pulseSpeed;
            float pulse = 1f + (Mathf.Sin(time) + 1f) * 0.5f * (pulseScale - 1f);
            rectTransform.localScale = originalScale * pulse;
            yield return null;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            IsInRange = false;
            detectionPopup.sprite = searchSprite;
            detectionPopup.enabled = false;
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
                detectionPopup.rectTransform.localScale = originalScale;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}