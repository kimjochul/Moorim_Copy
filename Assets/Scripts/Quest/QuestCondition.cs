using UnityEngine;

/// <summary>
/// 퀘스트 종류 enum
/// </summary>
public enum QuestEventType
{
    ItemChanged,
    Fail,
    ServeCustomer,
}

/// <summary>
/// 퀘스트에 대한 이벤트가 발생했을 때 어떤 이벤트인지 정의하기 위한 데이터 struct
/// </summary>
public struct QuestEvent
{
    public QuestEventType type;
    public int value; // 아이템이 필요한 퀘스트의 경우, itemID를 담는 값
    public int currentCount; // 퀘스트 진행 상황 카운트 값
}

/// <summary>
/// 퀘스트 완료 조건을 생성하기 위한 SO(퀘스트의 정보를 담고 있는 questdata에 변수 값으로 들어갈 SO를 생성하기 위한 클래스)
/// </summary>
public abstract class QuestCondition : ScriptableObject
{
    public abstract void Init(Inventory inventory);
    public abstract void Notify(QuestEvent questEvent);
    public abstract bool IsComplete();
    public abstract QuestCondition Clone();
}