using UnityEngine;
using TMPro;
using System.Collections;

public class BatteryPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float disappearTime = 1.0f;

    public void Show(int amount)
    {
        gameObject.SetActive(true);
        powerText.text = $"Power +{amount}";

        StopAllCoroutines();
        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Color startColor = powerText.color;

        while (elapsed < disappearTime)
        {
            elapsed += Time.deltaTime;

            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, elapsed / disappearTime);
            powerText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        transform.localPosition = startPos;
        gameObject.SetActive(false);
    }
}