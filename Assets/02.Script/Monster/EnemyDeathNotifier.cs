using UnityEngine;
public class EnemyDeathNotifier : MonoBehaviour
{
    private EnemySpawner spawner;
    private MapArea myArea;
    private bool isQuitting = false;

    public void Init(EnemySpawner spawner, MapArea area)
    {
        this.spawner = spawner;
        this.myArea = area;
    }

    private void OnApplicationQuit() { isQuitting = true; }

    private void OnDestroy()
    {
        if (!isQuitting && spawner != null && gameObject.scene.isLoaded)
        {
            spawner.OnMonsterDied(myArea);
        }
    }
}