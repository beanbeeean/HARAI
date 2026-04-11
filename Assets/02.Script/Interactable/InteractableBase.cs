using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction UI")]
    [SerializeField] protected GameObject interactionUI;

    // 플레이어가 직접 호출해주므로 OnTrigger 계열은 삭제하거나 비워둡니다.

    public virtual void ShowUI()
    {
        if (interactionUI != null) interactionUI.SetActive(true);
    }

    public virtual void HideUI()
    {
        if (interactionUI != null) interactionUI.SetActive(false);
    }

    public abstract void Interact();
}