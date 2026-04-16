using UnityEngine;

[System.Serializable]
public class SpawnPointState
{
    public Transform point;
    public GameObject currentItem;
    public float respawnTimer;
    public bool isCooling;
    public bool wasSpawning;

    public SpawnPointState(Transform point)
    {
        this.point = point;
    }
}

