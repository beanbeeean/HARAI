using UnityEngine;
using TMPro;
using System.Collections;

public class HealthPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float disappearTime = 1.0f;

    public void Show(int amount, bool isDamage = false)
    {
        gameObject.SetActive(true);

        if (healthText != null)
        {
            if (isDamage)
            {
                healthText.text = $"HP -{amount}";
                healthText.color = Color.red;
            }
            else
            {
                healthText.text = $"HP +{amount}";
                healthText.color = Color.green;
            }
        }

        StopAllCoroutines();
        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Color startColor = healthText.color;

        while (elapsed < disappearTime)
        {
            elapsed += Time.deltaTime;

            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, elapsed / disappearTime);
            healthText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        transform.localPosition = startPos;
        gameObject.SetActive(false);
    }
}