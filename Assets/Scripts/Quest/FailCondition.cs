using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/Fail")]
public class FailLimitCondition : QuestCondition
{
    public int maxFailCount;
    private int currentFailCount;

    public override void Init(Inventory inventory)
    {
        currentFailCount = 0;
    }

    public override void Notify(QuestEvent questEvent)
    {
        if (questEvent.type != QuestEventType.Fail)
            return;

        currentFailCount++;
    }

    public override bool IsComplete()
    {
        return currentFailCount <= maxFailCount;
    }

    public bool IsFailed()
    {
        return currentFailCount > maxFailCount;
    }
    
    public override QuestCondition Clone()
    {
        var instance = CreateInstance<FailLimitCondition>();
        instance.maxFailCount = maxFailCount;
        
        return instance;
    }
}