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
        //InitMessages();
    }

    private void InitMessages()
    {
        alertMessages.Add(AlertKey.Purifying, "정화를 완료했습니다...\n목표 수치 : {0}/{1}"); // o
        alertMessages.Add(AlertKey.GameClear, "모든 구역이 정화되었습니다... \n탈출하세요!"); // o
        alertMessages.Add(AlertKey.GameOver, "게임 오버... 정화 실패.."); // 이건 문구 생각좀.. && GameManager 만들어서 HPManager랑 연동 예정
        alertMessages.Add(AlertKey.GetCurse, "{0} 저주가 당신에게 스며듭니다."); // o
        alertMessages.Add(AlertKey.CleanseCurse, "스며든 저주가 해제되었습니다.");  // o
        alertMessages.Add(AlertKey.CannotGetCurse, "이미 저주가 가득합니다."); // o
        alertMessages.Add(AlertKey.CannotPickUp, "인벤토리가 이미 가득 찼습니다.");
        alertMessages.Add(AlertKey.CanCleanse, "이제 다시 저주를 해제할 수 있습니다."); // o
        alertMessages.Add(AlertKey.CannotCleanse, "{0}초 이후에 저주를 해제할 수 있습니다."); // o

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