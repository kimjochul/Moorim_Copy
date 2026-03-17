using UnityEngine;

public interface IObserver
{
    int QuestID { get; }
    string QuestName { get; }
    int CurrentCount { get; }
    bool IsCompleted { get; }
    void Notify(string questName);
    bool CanClear();
    void Complete();
}