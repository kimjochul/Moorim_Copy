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

    public void Init(Quest quest, Inventory inventory)
    {
        this.quest = quest;
        this.inventory = inventory;

        clearButton.onClick.RemoveAllListeners();
        clearButton.onClick.AddListener(OnClickClear);
    }

    
    private void OnClickClear()
    {
        if (!quest.CanClear())
            return;
        
        if (!quest.CanRemoveItems(inventory))
        {
            Debug.Log("아이템 부족");
            return;
        }
        
        quest.CostForQuestClear(inventory);
        // 완료 처리
        quest.Complete();
        gameObject.SetActive(false);
    }
}
