using UnityEngine;

public interface IObserver
{
    int QuestID { get; }
    string QuestName { get; }
    bool IsCompleted { get; }
    void Notify(QuestEvent questEvent);
    bool CanClear();
    void Complete();
}