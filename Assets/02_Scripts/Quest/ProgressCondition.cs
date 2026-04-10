using UnityEngine;
using static Constants;

/// <summary>
/// 게임 진행도에 따라 완료되는 퀘스트
/// </summary>
[CreateAssetMenu(menuName = "Quest/Condition/Item")]
public class ProgressCondition : QuestCondition
{
    private int currentMoney;
    private int currentFame;
    private int dayCount;

    public int requiredMoney;
    public int requiredFame;
    public int requiredDayCount;
    public QuestData Prerequisite_Quest; // 선행퀘스트는 퀘스트 매니저에서 완료된 퀘스트에서 id기반으로 찾아서 있으면 완료가능
    
    private DayManager dayManager;

    
    public override void Init(Inventory inventory = null)
    {
        dayManager = FindFirstObjectByType<DayManager>();
        
        currentMoney = EconomyManager.Instance.RestaurantEconomy.Money;
        currentFame = EconomyManager.Instance.RestaurantEconomy.Fame;
        dayCount = dayManager.DayData.day;
    }

    public override void Notify(QuestEvent questEvent, GuestMood mood = GuestMood.Neutral)
    {
        if(questEvent.type != QuestEventType.ProgressCheck)
            return;
        
        currentMoney = EconomyManager.Instance.RestaurantEconomy.Money;
        currentFame = EconomyManager.Instance.RestaurantEconomy.Fame;
        dayCount = dayManager.DayData.day;
        
        NotifyUpdated();
    }

    public override bool IsComplete()
    {
        if (currentMoney < requiredMoney)
        {
            Debug.Log("Not enough money");
            return false;
        }
        if (currentFame < requiredFame)
        {
            Debug.Log("Not enough fame");
            return false;
        }
        if (dayCount < requiredDayCount)
        {
            Debug.Log("Not enough day");
            return false;
        }
        if (Prerequisite_Quest != null)
        {
            if (!QuestManager.Instance.PrerequisiteQuestIsDone(Prerequisite_Quest.id))
                return false;
        }
        return true;
    }

    public override QuestCondition Clone()
    {
        var instance = CreateInstance<ProgressCondition>();
        instance.requiredMoney = requiredMoney;
        instance.requiredFame = requiredFame;
        instance.requiredDayCount = requiredDayCount;
        instance.Prerequisite_Quest = Prerequisite_Quest;
        
        return instance;
    }

    public override string GetDescription()
    {
        string description = "";

        if (requiredMoney != 0)
            description += $"요구 재력 : {requiredMoney}\n";
        
        if (requiredFame != 0)
            description += $"요구 명성치 : {requiredFame}\n";

        if (requiredDayCount != 0)
            description += $"{requiredDayCount}일 까지 진행\n";

        if (Prerequisite_Quest != null)
            description += $"선행퀘스트{Prerequisite_Quest.questName}";
        
        return description;
    }

    public override string GetProgressText()
    {
        string description = "";

        if (requiredMoney != 0)
            description += $"현재 재력 : {currentMoney}/{requiredMoney}\n";
        
        if (requiredFame != 0)
            description += $"요구 명성치 : {currentFame}/{requiredFame}\n";

        if (requiredDayCount != 0)
            description += $"현재 {dayCount}/{requiredDayCount}일 까지 진행";
        
        if (Prerequisite_Quest != null)
            description += $"선행퀘스트 : {Prerequisite_Quest.questName}";
        
        return description;
    }
}
