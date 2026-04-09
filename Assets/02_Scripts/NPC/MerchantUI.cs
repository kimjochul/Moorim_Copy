using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject talkPanel;

    [SerializeField] private Transform questContent;
    [SerializeField] private GameObject questSlotPrefab;
    
    [SerializeField] private Transform clearContent;
    [SerializeField] private GameObject clearSlotPrefab;
    
    private Merchant merchant;
    [SerializeField] private Shop shop;
    private Inventory inventory;
    private void OnEnable()
    {
        QuestManager.Instance.OnWaitingQuestChanged += RefreshAccept;
        QuestManager.Instance.OnActiveQuestChanged += RefreshClear;

        RefreshAccept();
        RefreshClear();
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnWaitingQuestChanged -= RefreshAccept;
        QuestManager.Instance.OnActiveQuestChanged -= RefreshClear;
        
        if (shop != null)
            shop.OnProductClicked -= HandleProductClicked;
    }

    public void Init(Merchant merchant)
    {
        this.merchant = merchant;
        shop.Init();
        
        shop.OnProductClicked += HandleProductClicked;
        
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Fridge);
        
        OpenTalk(); // 기본 탭
    }

    private void RefreshAccept()
    {
        var quests = QuestManager.Instance.GetWaitingQuests(QuestType.Merchant);

        // 기존 UI 삭제 (여기의 경우 acceptbar)
        foreach (Transform child in questContent)
            Destroy(child.gameObject);

        // 새로 생성
        foreach (var quest in quests)
        {
            var obj = Instantiate(questSlotPrefab, questContent);
            obj.GetComponent<QuestAcceptBar>().Init(quest);
        }
    }
    
    private void RefreshClear()
    {
        var quests = QuestManager.Instance.GetQuests(QuestType.Merchant);

        // 기존 UI 삭제 (여기의 경우 clearbar)
        foreach (Transform child in clearContent)
            Destroy(child.gameObject); // 만약 최적화 한다면 바꿔야 할 수도

        foreach (var quest in quests)
        {
            var obj = Instantiate(clearSlotPrefab, clearContent);
            obj.GetComponent<QuestClearBar>().Init(quest, inventory);
        }
    }
    
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        questPanel.SetActive(false);
        talkPanel.SetActive(false);
        
        shop.CreateCategory();
    }
    
    private void HandleProductClicked(ItemData itemData)
    {
        UIManager.Instance.CreateAmountPopup($"{itemData.itemName}을 구매하시겠습니까?", itemData,
            (amount) =>
            {
                int totalPrice = itemData.price * amount;
                
                if (!EconomyManager.Instance.CanSpend(totalPrice))
                {
                    UIManager.Instance.CreateOkPopup("돈이 부족합니다!", () => {});
                    return;
                }

                bool success = shop.Buy(itemData.id, amount);

                if (success)
                {
                    UIManager.Instance.CreateOkPopup("구매 완료!", () => {});
                }
            },
            () =>
            {
                Debug.Log("취소 버튼 눌림");
            });
    }
    
    public void OpenQuest()
    {
        shopPanel.SetActive(false);
        questPanel.SetActive(true);
        talkPanel.SetActive(false);
    }

    public void OpenTalk()
    {
        shopPanel.SetActive(false);
        questPanel.SetActive(false);
        talkPanel.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
