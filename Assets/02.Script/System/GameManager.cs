using TMPro;
using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private TextMeshProUGUI alertText;
    private CanvasGroup alertCanvasGroup;

    [Header("Exit Settings")]
    [SerializeField] private GameObject escapeDoor;

    private Coroutine currentFadeCoroutine; 

    private void Awake()
    {
        Instance = this;
        if (alertPanel != null)
        {
            alertCanvasGroup = alertPanel.GetComponent<CanvasGroup>();
            alertPanel.SetActive(false);
        }
    }

    public void UpdateMissionProgress(int current, int total)
    {
        string msg = $"정화를 완료했습니다...\n목표 수치 : {current}/{total}";
        ShowAlertSmooth(msg);
    }

    public void OnAllPurified()
    {
        ShowAlertSmooth("탈출 하세요!");
        if (escapeDoor != null) escapeDoor.SetActive(true);
    }

    public void ShowAlertSmooth(string message)
    {
        alertText.text = message;

        if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeRoutine(1f, 2.5f));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float delay)
    {
        alertPanel.SetActive(true);

        while (!Mathf.Approximately(alertCanvasGroup.alpha, targetAlpha))
        {
            alertCanvasGroup.alpha = Mathf.MoveTowards(alertCanvasGroup.alpha, targetAlpha, Time.deltaTime * 2f);
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        while (alertCanvasGroup.alpha > 0)
        {
            alertCanvasGroup.alpha -= Time.deltaTime * 1.5f;
            yield return null;
        }

        alertPanel.SetActive(false);
    }
}