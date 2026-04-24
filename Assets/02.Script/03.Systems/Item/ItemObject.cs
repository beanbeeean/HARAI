using System.Collections;
using UnityEngine;

public class ItemObject : InteractableBase
{
    public ItemData itemData;
    private PlayerInputReader playerInput;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float lifeTime = 45f;
    [SerializeField] private float blinkerTimer = 5f;
    [SerializeField] private float blinkerSpeed = 3f;

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

    public void DestoryItem()
    {
        StartCoroutine(DestroyRoutine());
    }
    
    IEnumerator DestroyRoutine()
    {
        Color itemColor = spriteRenderer.color;

        yield return new WaitForSeconds(lifeTime - blinkerTimer);
        float timer = 0f;

        while (timer <= blinkerTimer)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.PingPong(timer * blinkerSpeed, 1f);
            itemColor.a = alpha;
            spriteRenderer.color = itemColor;
            yield return null;
        }
        spriteRenderer.color = new Color(1f,1f,1f,1f);
        Destroy(gameObject);
    }    
}