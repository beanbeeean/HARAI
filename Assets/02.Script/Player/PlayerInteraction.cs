using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float detectionRadius = 1.2f;
    [SerializeField] private LayerMask interactionLayer;

    private bool isCooldown = false;
    private IInteractable currentInteractable;
    private void OnEnable()
    {
        if (inputReader != null)
            inputReader.InteractStartedEvent += OnInteract;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.InteractStartedEvent -= OnInteract;
    }

    private void Update()
    {
        ScanForInteractables();
    }

    private void ScanForInteractables()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, interactionLayer);

        if (hit != null && hit.TryGetComponent(out IInteractable interactable))
        {
            if (currentInteractable != interactable)
            {
                currentInteractable?.HideUI();
                currentInteractable = interactable;
                currentInteractable.ShowUI();
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.HideUI();
                currentInteractable = null;
            }
        }
    }

    private void OnInteract()
    {
        if (isCooldown || currentInteractable == null) return;

        StartCoroutine(InteractionCooldownRoutine());
        currentInteractable.Interact();
    }

    private IEnumerator InteractionCooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(0.3f);
        isCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}