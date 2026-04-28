using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Constants;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject questPanel;
    [SerializeField] private NPCTalkPanel talkPanel;
    [SerializeField] private DebtPanel debtPanel;

    [SerializeField] private NPCQuestUI npcQuestUI;
    
    [SerializeField] private Shop shop;
    private Inventory inventory;
    
    [SerializeField] private Button shopButton;
    [SerializeField] private Button questButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Button debtButton;
    [SerializeField] private Button exitButton;
    
    private bool isInitialized = false;
    
    [SerializeField] private string[] greetingDialogues;
    [SerializeField] private string[] idleDialogues; 
    
    private int lastDialogIndex = -1;
    
    public bool RepayDebtFinished { get; private set; } = false;
    
    private Merchant merchant;
    
    public enum MerchantState
    {
        Shop,
        Quest,
        Talk,
        Debt,
    }
    private MerchantState currentState;
    private void SetState(MerchantState state)
    {
        currentState = state;
        UpdateDialogue();
    }
    
    private void OnEnable()
    {
        lastDialogIndex = -1;
        
        debtPanel.FullPayDebt += FullyRepayDebt;
        
        OpenTalk();
    }

    private void Start()
    {
        OpenTalk();
    }

    private void OnDisable()
    {
        if (merchant.DayManager != null)
            merchant.DayManager.OnDayStart -= ResetDialogue;

        if (debtPanel != null)
            debtPanel.FullPayDebt -= FullyRepayDebt;
        
        if (shop != null)
            shop.OnProductClicked -= HandleProductClicked;
    }

    public void Init(Merchant merchant)
    {
        if (isInitialized) return;
        isInitialized = true;   
        
        this.merchant = merchant;
        
        shop.Init();
        
        if (merchant.IsAvailableDay())
            merchant.DayManager.OnDayStart += ResetDialogue;
        
        List<Constants.IngredientType> ingredientTypes = new List<Constants.IngredientType>();
        ingredientTypes.Add(Constants.IngredientType.Meats);
        ingredientTypes.Add(Constants.IngredientType.Vegetable);
        ingredientTypes.Add(Constants.IngredientType.Processed);
        ingredientTypes.Add(Constants.IngredientType.Spices);
        foreach(var a in ingredientTypes)
        {
            var ingredientList = DataManager.Instance.GetIngredientList(a);
            foreach (var ingredientData in ingredientList)
            {
                Item item = ItemFactory.Instance.CreateItem(ingredientData.id);
                
                if(ingredientData.type == Constants.IngredientType.Meats)
                    InventoryManager.Instance.GetInventory(InventoryType.IngredientsButMeatOnly).GetItem(item, 10);
                else
                    InventoryManager.Instance.GetInventory(InventoryType.Ingredients).GetItem(item, 20);
            }
        }
        
        shop.OnProductClicked += HandleProductClicked;
        
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Fridge);
        
        shopButton.onClick.AddListener(OpenShop);
        questButton.onClick.AddListener(OpenQuest);
        talkButton.onClick.AddListener(OpenTalk);
        debtButton.onClick.AddListener(OpenDebt);
        exitButton.onClick.AddListener(Close);
        
        currentState = MerchantState.Talk;
    }
    
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        questPanel.SetActive(false);
        talkPanel.gameObject.SetActive(true);
        debtPanel.gameObject.SetActive(false);
        
        SetState(MerchantState.Shop);
    }
    
    private void HandleProductClicked(ItemData itemData)
    {
        talkPanel.ChangeText($"{itemData.itemName}을 사고 싶어?");
        UIManager.Instance.CreateAmountPopup($"{itemData.itemName}을 구매하시겠습니까?", itemData,
            (amount) =>
            {
                int totalPrice = itemData.price * amount;
                
                if (!EconomyManager.Instance.CanSpend(totalPrice))
                {
                    talkPanel.ChangeText("돈이 부족한 것 같은데?");
                    UIManager.Instance.CreateOkPopup("돈이 부족합니다!", () => {});
                    return;
                }

                bool success = shop.Buy(itemData.id, amount);

                if (success)
                {
                    talkPanel.ChangeText("좋은 선택야!");
                    UIManager.Instance.CreateOkPopup("구매 완료!", () => {});
                }
            },
            () =>
            {
                talkPanel.ChangeText("천천히 골라도 돼");
                Debug.Log("취소 버튼 눌림");
            });
    }
    
    public void OpenQuest()
    {
        shopPanel.SetActive(false);
        questPanel.SetActive(true);
        talkPanel.gameObject.SetActive(true);
        debtPanel.gameObject.SetActive(false);
        
        npcQuestUI.Init(QuestType.Merchant, NPCType.Merchant);
        
        SetState(MerchantState.Quest);
    }

    public void OpenTalk()
    {
        talkPanel.gameObject.SetActive(false);
        shopPanel.SetActive(false);
        questPanel.SetActive(false);
        talkPanel.gameObject.SetActive(true);
        debtPanel.gameObject.SetActive(false);

        talkPanel.Clear();
        
        SetState(MerchantState.Talk);
    }
    
    public void OpenDebt()
    {
        shopPanel.SetActive(false);
        questPanel.SetActive(false);
        talkPanel.gameObject.SetActive(true);
        debtPanel.gameObject.SetActive(true);

        SetState(MerchantState.Debt);
    }
    
    private void UpdateDialogue()
    {
        string dialogue = "";

        switch (currentState)
        {
            case MerchantState.Shop:
                dialogue = "좋은 물건 많아~ 구경하고 가";
                break;

            case MerchantState.Quest:
                dialogue = "일 좀 부탁해도 될까? 보상은 할게";
                break;

            case MerchantState.Talk:
                dialogue = GetTalkDialogue();
                break;
            
            case MerchantState.Debt:
                dialogue = "돈을 갚고 싶어?";
                break;

            default:
                dialogue = "";
                break;
        }
        //Debug.Log($"대사 출력: {dialogue}");
        talkPanel.ChangeText(dialogue);
    }

    private string GetRandomDialogue(string[] dialogues)
    {
        if (dialogues == null || dialogues.Length == 0)
            return "";

        int index;

        do
        {
            index = Random.Range(0, dialogues.Length);
        }
        while (dialogues.Length > 1 && index == lastDialogIndex);

        lastDialogIndex = index;
        return dialogues[index];
    }
    
    private string GetTalkDialogue()
    {
        // 30% 확률로 인사, 70% 잡담
        if (Random.value < 0.3f)
        {
            return GetRandomDialogue(greetingDialogues);
        }
        else
        {
            return GetRandomDialogue(idleDialogues);
        }
    }

    private void FullyRepayDebt()
    {
        RepayDebtFinished = true;
        debtPanel.gameObject.SetActive(false);
        debtButton.gameObject.SetActive(false);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
        DayManager dayManager = FindFirstObjectByType<DayManager>();
        dayManager.DayStart();
    }
    
    public void ResetDialogue()
    {
        lastDialogIndex = -1;
    }
}
