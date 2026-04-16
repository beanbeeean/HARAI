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
}

[System.Serializable]
public struct AlertEntry
{
    public AlertKey key;
    public string message;
}
