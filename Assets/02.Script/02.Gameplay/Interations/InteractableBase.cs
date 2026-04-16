using UnityEngine;
using System.Collections;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction UI")]
    [SerializeField] protected GameObject interactionUI;

    [Header("Animation Settings")]
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 5f;
    [SerializeField] private float fadeDuration = 0.1f;

    private CanvasGroup canvasGroup;
    private Vector3 initialLocalPos;
    private Coroutine currentFadeRoutine;

    protected virtual void Awake()
    {
        if (interactionUI != null)
        {
            canvasGroup = interactionUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = interactionUI.AddComponent<CanvasGroup>();

            initialLocalPos = interactionUI.transform.localPosition;

            canvasGroup.alpha = 0f;
            interactionUI.SetActive(false);
        }
    }

    protected virtual void Update()
    {
        if (interactionUI != null && interactionUI.activeSelf)
        {
            float newY = initialLocalPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            interactionUI.transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);

            interactionUI.transform.rotation = Quaternion.identity;
        }
    }

    public virtual void ShowUI()
    {
        if (interactionUI == null) return;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        interactionUI.SetActive(true);
        currentFadeRoutine = StartCoroutine(FadeRoutine(1f));
    }

    public virtual void HideUI()
    {
        if (interactionUI == null) return;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(FadeRoutine(0f));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (targetAlpha <= 0) interactionUI.SetActive(false);
    }

    public abstract void Interact();
}