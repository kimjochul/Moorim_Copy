using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private Transform questAcceptUIParent;
    [SerializeField] private Transform questClearUIParent;

    [SerializeField] private GameObject questAcceptPrefab;
    [SerializeField] private GameObject questClearPrefab;
    
    public void CreateAcceptUI(Quest quest)
    {
        GameObject obj = Instantiate(questAcceptPrefab, questAcceptUIParent);
        obj.GetComponent<QuestAcceptBar>().Init(quest);
    }

    public void CreateClearUI(Quest quest, Inventory inventory)
    {
        GameObject obj = Instantiate(questClearPrefab, questClearUIParent);
        obj.GetComponent<QuestClearBar>().Init(quest, inventory);
    }
}
