using System.Collections.Generic;
using UnityEngine;

public class Quest : IObserver
{
    private QuestData data;
    public QuestData Data => data;
    
    public int QuestID { get; }
    public string QuestName { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsFailed { get; private set; }

    private List<QuestCondition> conditions;
    public Quest(QuestData questData)
    {
        data = questData;
        QuestID = data.id;
        QuestName = data.questName;
        IsCompleted = false;
        
        conditions = new List<QuestCondition>();
        var inventory = Object.FindFirstObjectByType<Inventory>();

        foreach (var condition in data.conditions)
        {
            var instance = condition.Clone();
            instance.Init(inventory);
                
            conditions.Add(instance);
        }
        
        QuestManager.Instance.AddObserver(this);
    }
    
    public void Notify(QuestEvent questEvent)
    {
        if (IsCompleted || IsFailed)
            return;

        foreach (var condition in conditions)
        {
            condition.Notify(questEvent);
            
            if (condition is FailLimitCondition failCondition && failCondition.IsFailed())
            {
                Fail();
                return;
            }
        }

        if (CanClear())
        {
            Complete();
        }
    }

    public bool CanClear()
    {
        foreach (var condition in conditions)
        {
            if (!condition.IsComplete())
                return false;
        }
        return true;
    }
    
    public bool CanRemoveItems(Inventory inventory)
    {
        foreach (var condition in conditions)
        {
            if (condition is ItemCondition itemCondition)
            {
                int have = inventory.GetItemCount(itemCondition.itemID);
                if (have < itemCondition.requiredCount)
                    return false;
            }
        }
        return true;
    }

    public void CostForQuestClear(Inventory inventory)
    {
        foreach (var condition in conditions)
        {
            if (condition is ItemCondition itemCondition)
            {
                inventory.RemoveItem(itemCondition.itemID, itemCondition.requiredCount);
            }
        }
    }
    
    public void Complete()
    {
        IsCompleted = true;
        Debug.Log($"퀘스트 완료: {QuestName}");
        QuestManager.Instance.RemoveObserver(this);
    }
    
    public void Fail()
    {
        IsFailed = true;
        Debug.Log($"퀘스트 실패: {QuestName}");
        QuestManager.Instance.RemoveObserver(this);
    }
}
