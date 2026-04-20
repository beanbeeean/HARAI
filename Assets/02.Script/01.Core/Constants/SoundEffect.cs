using UnityEngine;

public enum SoundType
{
    Door,
    Hit,
    Purificate,
    PickDropItem,
    Flashlight,
    Teleport,
    UseBrokenGlass,
    Ending,
    EnemyLaugh_1,
    EnemyLaugh_2,
    PlayerDied,
    EnemyScream,
    TitleBGM,
    GameBGM
}

[System.Serializable]
public class SoundEntry
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float defaultVolume;
}