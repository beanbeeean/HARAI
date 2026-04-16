using UnityEngine;

[CreateAssetMenu(fileName = "NewCurse", menuName = "CurseObjects/Curse")]
public class CurseData : ScriptableObject
{
    public string curseID;
    public string curseName;
    public string curseDescription;
    public CurseType type;
    public float value;
    
}