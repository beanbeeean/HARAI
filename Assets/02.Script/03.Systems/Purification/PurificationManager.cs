using System;
using System.Collections;
using UnityEngine;

public class PurificationManager : MonoBehaviour
{
    public static PurificationManager Instance;

    public int TotalTargets { get; private set; }
    public int CurrentPurified { get; private set; }

    public Action gameClearEvent;
    private void Awake() => Instance = this;

    private void Start()
    {
        TotalTargets = FindObjectsByType<PurificationObject>(FindObjectsSortMode.None).Length;
        Debug.Log($"TotalTargets : {TotalTargets}");

        // StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(5f);
        gameClearEvent?.Invoke();
    }

    public void OnObjectPurified()
    {
        CurrentPurified++;
        InfoManager.Instance.UpdatePurifyInfo(CurrentPurified);
        AlertManager.Instance.ShowAlert(AlertKey.Purifying, CurrentPurified, TotalTargets);

        if (CurrentPurified >= TotalTargets)
        // if (CurrentPurified >= 2)
        {
            // AlertManager.Instance.ShowAlert(AlertKey.GameClear);
            gameClearEvent?.Invoke();
        }
    }
}