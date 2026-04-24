using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    private EnemySpawner spawner;
    private MapArea myArea;
    private bool isQuitting = false;
    private bool isDeadReported = false;

    public void Init(EnemySpawner spawner, MapArea area)
    {
        this.spawner = spawner;
        this.myArea = area;
        isDeadReported = false;
    }

    private void OnApplicationQuit() { isQuitting = true; }

    private void OnDisable() 
    {
        if (!isQuitting && spawner != null && !isDeadReported)
        {
            isDeadReported = true;
            spawner.OnMonsterDied(myArea);
        }
    }
}