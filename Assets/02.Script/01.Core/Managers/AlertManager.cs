using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance;

    [Header("Alert DB")]
    [SerializeField] private List<AlertEntry> alertDataList = new List<AlertEntry>();

    [Header("UI References")]
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private TextMeshProUGUI alertText;
    private CanvasGroup alertCanvasGroup;

    private Dictionary<AlertKey, string> alertMessages = new Dictionary<AlertKey, string>();
    private Queue<string> alertMessagesQueue = new Queue<string>();

    [SerializeField] private bool isShowing = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(AlertEntry entry in alertDataList)
        {
            if (!alertMessages.ContainsKey(entry.key))
            {
                alertMessages.Add(entry.key, entry.message);
            }
        }

        if (alertPanel != null)
        {
            alertCanvasGroup = alertPanel.GetComponent<CanvasGroup>();
            alertPanel.SetActive(false);
        }
        
    }


    public void ShowAlert(AlertKey key, params object[] args)
    {
        if(alertMessages.TryGetValue(key, out string msg))
        {
            string showMsg = string.Format(msg, args);
            alertMessagesQueue.Enqueue(showMsg);
        }

        if (!isShowing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;
        while (alertMessagesQueue.Count > 0)
        {
            string next = alertMessagesQueue.Dequeue();
            alertText.text = next;
            yield return StartCoroutine(FadeRoutine(1f, 2.5f));

        }
        isShowing = false;
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