using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] Image detectionPopup;
    [SerializeField] private Color farColor;
    [SerializeField] private Color nearColor;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;

    [SerializeField] float swingAngle = 15f;
    [SerializeField] float swingSpeed = 5f; 

    private Coroutine swingCoroutine;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            detectionPopup.enabled = true;
            if (swingCoroutine == null)
            {
                swingCoroutine = StartCoroutine(SwingPopup());
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            float distance = Vector2.Distance(transform.position, collision.transform.position);

            float percent = (maxDistance - distance) / (maxDistance - minDistance);
            percent = Mathf.Clamp01(percent);

            detectionPopup.color = Color.Lerp(farColor, nearColor, percent);


        }
    }

    IEnumerator SwingPopup()
    {
        RectTransform popupRectTransform = detectionPopup.rectTransform;
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * swingSpeed;
            float angle = Mathf.Sin(time) * swingAngle;
            popupRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null; 
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Purification"))
        {
            detectionPopup.enabled = false;
            if (swingCoroutine != null)
            {
                StopCoroutine(swingCoroutine);
                swingCoroutine = null;
                detectionPopup.rectTransform.rotation = Quaternion.identity;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
