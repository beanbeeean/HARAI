public enum AlertKey
{
    Purifying,
    GetCurse,
    CleanseCurse,
    CannotGetCurse,
    GameClear,
    GameOver,
    CannotPickUp,
    CanCleanse,
    CannotCleanse,
    NoCurse,
    FullBattery,
    FullHealth,
    Stun,
    Immune,
    NoImmune,
    StartMsg_1,
    StartMsg_2,
    StartMsg_3,
    StartMsg_4,
}

[System.Serializable]
public struct AlertEntry
{
    public AlertKey key;
    public string message;
}
