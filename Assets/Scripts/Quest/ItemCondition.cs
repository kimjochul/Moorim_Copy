using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/Item")]
public class ItemCondition : QuestCondition
{
    public int itemID;
    public int requiredCount;

    private int currentCount;

    public override void Init(Inventory inventory)
    {
        currentCount = inventory.GetItemCount(itemID);
    }

    public override void Notify(QuestEvent questEvent)
    {
        if (questEvent.type != QuestEventType.ItemGet)
            return;

        if (questEvent.value != itemID)
            return;

        currentCount++;
    }

    public override bool IsComplete()
    {
        return currentCount >= requiredCount;
    }
    
    public override QuestCondition Clone()
    {
        var instance = CreateInstance<ItemCondition>();
        instance.itemID = itemID;
        instance.requiredCount = requiredCount;
        
        return instance;
    }
}