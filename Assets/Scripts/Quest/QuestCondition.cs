using UnityEngine;

public enum QuestEventType
{
    ItemGet,
    Fail,
    ServeCustomer,
}

public abstract class QuestCondition : ScriptableObject
{
    public abstract void Init(Inventory inventory);
    public abstract void Notify(QuestEvent questEvent);
    public abstract bool IsComplete();
    public abstract QuestCondition Clone();
}


public struct QuestEvent
{
    public QuestEventType type;
    public int value; // itemID or 기타 값
}