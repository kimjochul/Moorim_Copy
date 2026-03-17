using UnityEngine;
using UnityEngine.UI;

public class QuestClearBar : MonoBehaviour
{
    [SerializeField] private Button clearButton;
    [SerializeField] private Inventory inventory;
    
    private Quest quest;

    private void OnEnable()
    {
        inventory = FindFirstObjectByType<Inventory>();
    }

    public void Init(Quest quest)
    {
        this.quest = quest;
        clearButton.onClick.AddListener(OnClickClear);
    }

    private void OnClickClear()
    {
        if (!quest.CanClear())
            return;

        foreach (var item in quest.Data.requireItems)
        {
            int have = inventory.GetItemCount(item.questItem);
        
            if (have < item.requestCount)
            {
                Debug.Log("아이템이 부족합니다");
                return;
            }
            
            inventory.RemoveItem(item.questItem, item.requestCount);
        }
        
        quest.Complete();
        gameObject.SetActive(false);
    }
}
