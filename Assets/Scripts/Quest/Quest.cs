using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 퀘스트 객체에 대한 클래스
/// </summary>
public class Quest : IObserver
{
    private QuestData data;
    public QuestData Data => data;
    
    public int QuestID { get; }
    public string QuestName { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsFailed { get; private set; }

    private List<QuestCondition> conditions;
    
    /// <summary>
    /// 퀘스트를 생성하는 생성자, 퀘스트 데이터를 받아서 퀘스트를 생성
    /// </summary>
    /// <param name="questData"></param>
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
    
    /// <summary>
    /// 관찰자로 부터 퀘스트 진행상황에 대한 업데이트 공지(QuestEvent의 정보)가 왔을 때 본인의 퀘스트가 진행된 건지 확인하는 함수
    /// </summary>
    /// <param name="questEvent"></param>
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

    /// <summary>
    /// 클리어 가능한 상황인지 여부를 확인하는 함수
    /// </summary>
    /// <returns></returns>
    public bool CanClear()
    {
        foreach (var condition in conditions)
        {
            if (!condition.IsComplete())
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// 아이템을 인벤토리에서 현재 갖고 있는 양을 확인하고 제출 가능한지 확인하는 함수(퀘스트 클리어 시, 아이템 제출을 할 때 사용)
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 아이템을 제출하는 퀘스트의 경우, 퀘스트 완료 시에 아이템을 인벤토리에서 지워달라고 요청하는 함수
    /// </summary>
    /// <param name="inventory"></param>
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
    
    /// <summary>
    ///  퀘스트 완료시 호출되는 함수, 퀘스트가 성공됐다고 bool값 갱신
    /// </summary>
    public void Complete()
    {
        IsCompleted = true;
        Debug.Log($"퀘스트 완료: {QuestName}");
        QuestManager.Instance.RemoveObserver(this);
    }
    
    /// <summary>
    /// 퀘스트 실패 시 호출되는 함수, 퀘스트 실패의 bool값 갱신
    /// </summary>
    public void Fail()
    {
        IsFailed = true;
        Debug.Log($"퀘스트 실패: {QuestName}");
        QuestManager.Instance.RemoveObserver(this);
    }
}
