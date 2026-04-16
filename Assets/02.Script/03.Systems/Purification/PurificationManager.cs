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
        AlertManager.Instance.ShowAlert(AlertKey.Purifying, CurrentPurified, TotalTargets);

        if (CurrentPurified >= TotalTargets)
        {
            AlertManager.Instance.ShowAlert(AlertKey.GameClear);
        }
    }
}