using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float detectionRadius = 1.2f;
    [SerializeField] private LayerMask interactionLayer;

    private bool isCooldown = false;
    private IInteractable currentInteractable; // 현재 범위 내에 감지된 물체

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
        // 매 프레임 주변을 스캔하여 UI 제어 및 대상 지정
        ScanForInteractables();
    }

    private void ScanForInteractables()
    {
        // 1. Detection Radius 이내의 물체 탐색
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, interactionLayer);

        if (hit != null && hit.TryGetComponent(out IInteractable interactable))
        {
            // 새로운 대상을 찾았거나, 대상이 바뀐 경우
            if (currentInteractable != interactable)
            {
                // 이전 대상의 UI를 끄고 새 대상의 UI를 켬
                currentInteractable?.HideUI();
                currentInteractable = interactable;
                currentInteractable.ShowUI();
            }
        }
        else
        {
            // 범위 내에 아무것도 없음
            if (currentInteractable != null)
            {
                currentInteractable.HideUI();
                currentInteractable = null;
            }
        }
    }

    private void OnInteract()
    {
        // 쿨타임 중이거나 범위 내에 대상이 없으면 리턴
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