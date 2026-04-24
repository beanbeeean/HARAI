using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [SerializeField] private GameObject enemyPrefab;
    private IObjectPool<GameObject> pool;

    [SerializeField] MapArea[] mapAreas = new MapArea[2];
    private int capacity;

    private void Awake()
    {
        Instance = this;
        pool = new ObjectPool<GameObject>(
            createFunc: CreateEnemy,
            actionOnGet: OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: true, 
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private GameObject CreateEnemy() => Instantiate(enemyPrefab, transform);
    private void OnGetEnemy(GameObject enemy) {
        enemy.SetActive(true);
    }
    private void OnReleaseEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }
    private void OnDestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
    }

    public GameObject Get(Vector3 spawnPosition)
    {
        GameObject enemy = pool.Get();
        
        if (enemy.TryGetComponent(out CommonMonster monster))
        {
            // 상태를 리셋하는 함수 만들어야 됨.
        }
        
        return enemy;
    }

    public void Release(GameObject enemy)
    {
        pool.Release(enemy);
    }
}