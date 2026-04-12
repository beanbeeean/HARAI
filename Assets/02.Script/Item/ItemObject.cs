using UnityEngine;

public class ItemObject : InteractableBase
{
    public ItemData itemData;
    private PlayerInputReader playerInput;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInput = collision.GetComponent<PlayerInputReader>();
            if (playerInput != null)
            {
                playerInput.PickUpStartedEvent += Interact;
            }
            ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerInput != null)
            {
                playerInput.PickUpStartedEvent -= Interact;
                playerInput = null;
            }
            HideUI();
        }
    }

    public override void Interact()
    {
        InventoryManager inv = playerInput.GetComponent<InventoryManager>();

        if (inv != null && itemData != null)
        {
            if (inv.AddItem(itemData))
            {
                if (playerInput != null) playerInput.PickUpStartedEvent -= Interact;
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null) playerInput.PickUpStartedEvent -= Interact;
    }
}