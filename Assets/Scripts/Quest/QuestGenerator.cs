using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    [SerializeField] private QuestPool questPool;
    [SerializeField] private Inventory inventory;
    
    /// <summary>
    /// 해금이 되어있는 아이템을 필요로 하는 퀘스트만 나오게 설정해야함
    /// 퀘스트 풀에 있는 데이터들 중 하나를 랜덤으로 골라서, 리턴
    /// </summary>
    public Quest GenerateRandomQuest()
    {
        if (questPool.quests.Count == 0)
            return null;
        
        QuestData template = questPool.quests[Random.Range(0, questPool.quests.Count)];
        
        // 여기에서 퀘스트 필요 아이템이 언락이 되어있는지 확인
        
        Quest quest = new Quest(template, inventory);
        
        RandomizeQuest(quest);

        Debug.Log("id : " + quest.Data.id);
        Debug.Log("questName : " + quest.Data.questName);
        Debug.Log("quest condition Name : " + quest.Data.conditions[0]);
        
        return quest;
    }
    
    /// <summary>
    /// 퀘스트에 있는 퀘스트 조건의 퀘스트 조건들의 완료 개수 랜덤 설정
    /// </summary>
    /// <param name="quest"></param>
    public void RandomizeQuest(Quest quest)
    {
        foreach (var condition in quest.Conditions)
        {
            if (condition is ItemCondition item)
            {
                item.requiredCount = Random.Range(1, 1);
                Debug.Log("quest condition current count : " + item.requiredCount);
            }

            if (condition is FailLimitCondition fail)
            {
                fail.maxFailCount = Random.Range(1, 5);
            }
        }
    }
}
