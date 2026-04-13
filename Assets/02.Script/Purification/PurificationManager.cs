using UnityEngine;

public class PurificationManager : MonoBehaviour
{
    public static PurificationManager Instance;

    public int TotalTargets { get; private set; }
    public int CurrentPurified { get; private set; }

    private void Awake() => Instance = this;

    private void Start()
    {
        TotalTargets = FindObjectsByType<PurificationObject>(FindObjectsSortMode.None).Length;
        Debug.Log($"TotalTargets : {TotalTargets}");
    }

    public void OnObjectPurified()
    {
        CurrentPurified++;
        GameManager.Instance.UpdateMissionProgress(CurrentPurified, TotalTargets);

        if (CurrentPurified >= TotalTargets)
        {
            GameManager.Instance.OnAllPurified();
        }
    }
}