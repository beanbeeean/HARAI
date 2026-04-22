using UnityEngine;

public enum SoundType
{
    None,
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
    GameBGM,
    HittingGround_Story,
    Footstep_Story,
    DoorBang_Story,
    DropBag_Story,
    DoorOpening_Story,
    DoorClosing_Story,
    StoryBGM
}

[System.Serializable]
public class SoundEntry
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float defaultVolume;
}