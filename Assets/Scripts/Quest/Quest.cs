using System.Collections.Generic;
using UnityEngine;

public class Quest : IObserver
{
    private QuestData data;
    public QuestData Data => data;
    
    public int QuestID { get; }
    public string QuestName { get; private set; }
    public int CurrentCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsFailed { get; private set; }

    private Dictionary<string, int> itemCountMap;
    public Dictionary<string, int> ItemCountMap => itemCountMap;
    
    public Quest(QuestData questData)
    {
        data = questData;
        QuestID = data.id;
        QuestName = data.questName;
        IsCompleted = false;
        
        itemCountMap = new Dictionary<string, int>();

        foreach (var item in data.requireItems)
        {
            itemCountMap[item.questItem] = 0; // 인벤토리 만들면 여기에 값 넣어주면 됨
        }
        
        QuestManager.Instance.AddObserver(this);
    }
    
    public void Notify(string questItemName)
    {
        if (IsCompleted)
            return;

        if (!itemCountMap.ContainsKey(questItemName))
            return;

        itemCountMap[questItemName]++;

        if (CanClear())
        {
            Complete();
        }
    }

    public bool CanClear()
    {
        foreach (var require in data.requireItems)
        {
            if (itemCountMap[require.questItem] < require.requestCount)
                return false;
        }
        return true;
    }

    public void Complete()
    {
        IsCompleted = true;
        Debug.Log($"퀘스트 완료: {QuestName}");
    }
}
