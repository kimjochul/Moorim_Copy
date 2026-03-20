using UnityEngine;

/// <summary>
/// 추적 대상의 인터페이스
/// </summary>
public interface IObserver
{
    int QuestID { get; }
    string QuestName { get; }
    bool IsCompleted { get; }
    void Notify(QuestEvent questEvent);
    bool CanClear();
    void Complete();
}