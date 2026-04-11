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
        // 게임 종료 중이 아니고, Spawner가 살아있을 때만 리스폰 요청
        if (!isQuitting && spawner != null && gameObject.scene.isLoaded)
        {
            spawner.OnMonsterDied(myArea);
        }
    }
}