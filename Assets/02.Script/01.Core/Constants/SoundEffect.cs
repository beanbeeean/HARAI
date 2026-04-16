using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float defaultVolume;
}