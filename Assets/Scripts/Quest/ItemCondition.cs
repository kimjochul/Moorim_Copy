using System;
using UnityEngine;

/// <summary>
/// 아이템 제출을 필요로 하는 퀘스트 조건에 대한 클래스
/// </summary>
[CreateAssetMenu(menuName = "Quest/Condition/Item")]
public class ItemCondition : QuestCondition
{
    public int itemID;
    public int requiredCount;

    private int currentCount;

    /// <summary>
    /// 아이템 제출 퀘스트는 인벤토리가 필요하니 init
    /// </summary>
    /// <param name="inventory"></param>
    public override void Init(Inventory inventory)
    {
        currentCount = inventory.GetItemCount(itemID);
    }

    /// <summary>
    /// 만약 questevent가 왔을 때 questeventtype이 itemchanged일 경우, 본인의 퀘스트가 필요로 하는 아이템인지 확인 후 값 갱신.   
    /// </summary>
    /// <param name="questEvent"></param>
    public override void Notify(QuestEvent questEvent)
    {
        if (questEvent.type != QuestEventType.ItemChanged)
            return;

        if (questEvent.ID != itemID)
            return;

        currentCount = questEvent.currentCount;
    }
    
    /// <summary>
    /// 퀘스트 조건이 현재 완료 가능한 상황인 지 체크할 때 호출되는 함수 
    /// </summary>
    /// <returns></returns>
    public override bool IsComplete()
    {
        return currentCount >= requiredCount;
    }
    
    /// <summary>
    /// 퀘스트의 조건을 퀘스트 파일의 생성자에서 등록할 때, 본인 조건을 클로닝해서 값으로 전달하여서 생성자 안에서 등록이 가능하게 하는 함수
    /// </summary>
    /// <returns></returns>
    public override QuestCondition Clone()
    {
        var instance = CreateInstance<ItemCondition>();
        instance.itemID = itemID;
        instance.requiredCount = requiredCount;
        
        return instance;
    }
}