using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: 퀘스트 보상관련 추가

/// <summary>
/// 퀘스트 객체에 대한 클래스
/// Condition 리스트 보관
/// Condition에게 이벤트 전달
/// 전체 완료 여부 판단
/// Complete / Fail 처리
/// </summary>
public class Quest : IObserver
{
    private QuestData data;
    public QuestData Data => data;
    public QuestType QuestType => data.questType;
    
    public int QuestID { get; }
    public string QuestName { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsAccepted  { get; private set; }
    public bool IsFailed { get; private set; }

    public QuestReward RuntimeReward => data.reward;
    public List<int> RewardRecipeIds;
    public List<int> RewardIngredientIds;
    
    public NPCType targetNPC; // 퀘스트 완료 할 수 있는 대상
    
    private List<QuestCondition> conditions;
    public List<QuestCondition> Conditions => conditions;
    
    private Inventory inventory;
    
    /// <summary>
    /// 퀘스트를 생성하는 생성자, 퀘스트 데이터를 받아서 퀘스트를 생성
    /// </summary>
    /// <param name="questData"></param>
    public Quest(QuestData questData, Inventory inventory)
    {
        data = questData;
        QuestID = data.id;
        QuestName = data.questName;
        IsCompleted = false;
        IsAccepted = false;
        
        targetNPC = questData.npcType;
        this.inventory = inventory;
        
        conditions = new List<QuestCondition>();
        foreach (var condition in data.conditions)
        {
            var instance = condition.Clone();
            instance.Init(inventory);
                
            conditions.Add(instance);
        }

        RewardRecipeIds = questData.rewardRecipeIds;
        RewardIngredientIds = questData.rewardIngredientIds;
    }
    
    /// <summary>
    /// 관찰자로 부터 퀘스트 진행상황에 대한 업데이트 공지(QuestEvent의 정보)가 왔을 때 본인의 퀘스트가 진행된 건지 확인하는 함수
    /// </summary>
    /// <param name="questEvent"></param>
    public void Notify(QuestEvent questEvent, Constants.GuestMood mood = Constants.GuestMood.Neutral)
    {
        if (IsCompleted || IsFailed)
            return;
        
        Debug.Log(questEvent.ToString());
        
        foreach (var condition in conditions)
        {
            condition.Notify(questEvent, mood);
            
            if (condition is FailLimitCondition failCondition && failCondition.IsFailed())
            {
                Fail();
                return;
            }
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
        
        EconomyManager.Instance.AddMoney(RuntimeReward.gold);
        EconomyManager.Instance.AddFame(RuntimeReward.reputation);
        
        QuestManager.Instance.CompletedQuests.Add(this);
        QuestManager.Instance.EraseQuestFromActive(this);
        QuestManager.Instance.RemoveObserver(this);
        
        if (QuestType == QuestType.SubStory)
        {
            SubStoryQuestController.Instance.OnQuestCompleted(this, inventory);
        }
        
        if(Data.questType == QuestType.MainStory)
            MainStoryQuestController.Instance.StartNextQuest(inventory);
    }
    
    /// <summary>
    /// 퀘스트 실패 시 호출되는 함수, 퀘스트 실패의 bool값 갱신
    /// </summary>
    public void Fail()
    {
        IsFailed = true;
        Debug.Log($"퀘스트 실패: {QuestName}");
        QuestManager.Instance.EraseQuestFromActive(this);
        QuestManager.Instance.RemoveObserver(this);
    }
    
    /// <summary>
    /// 퀘스트 조건들 호출(주로 진행도 확인 용)
    /// </summary>
    /// <returns></returns>
    public List<QuestCondition> GetConditions()
    {
        return conditions;
    }

    /// <summary>
    /// 퀘스트 수락 시 프로퍼티 체크
    /// </summary>
    public void QuestAcceptCheck()
    {
        IsAccepted = true;
    }
}
