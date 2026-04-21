using UnityEngine;

public enum SpeakerType
{
    Player,
    Enemy
}

[System.Serializable]
public class DialogueEntry
{
    public SpeakerType speakerType;
    public string speakerName;
    public string dialogueText;
    public SoundType soundType;
}
