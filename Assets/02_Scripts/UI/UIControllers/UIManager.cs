using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;
    [SerializeField] private GameObject questPopupObj;
    [SerializeField] private GameObject orderPopupObj;
    [SerializeField] private GameObject amountPopupObj;

    [SerializeField] private Button _questButton;
    [SerializeField] private Button _orderButton;
    
    [SerializeField] QuestPopupController _questPopupCtr;
    [SerializeField] OrdersPopupController _orderPopupCtr;
    
    private bool _isQuestPopupOpen = false;
    private bool _isOrderPopupOpen = false;
    
    // temp 
    private Dictionary<Recipe, int> tempRecipes;
    [SerializeField] private GuestData tempGuestData;
    [SerializeField] private List<RecipeData> recipes;
    List<Order> tempOrders = new List<Order>();
    
    protected override void Awake()
    {
        base.Awake();
        _canvas = FindFirstObjectByType<Canvas>();
        
        _questButton.onClick.AddListener(OpenQuestPopup);
        _orderButton.onClick.AddListener(OpenOrdersPopup);
        
        // temp
        foreach (var recipe in recipes)
        {
            Recipe newRecipe = new Recipe(recipe);
            Dictionary<Recipe, int> order = new  Dictionary<Recipe, int>();
            order.Add(newRecipe, Random.Range(0, 3));
            tempOrders.Add(new Order(order, tempGuestData));
        }
    }
    
    // 스토리에서 대화창 열기
    public void OpenDialogPopup(string characterName, string dialog)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(characterName, dialog);
    }
    
    // 게임씬의 오른쪽 위 버튼 누르면 나오는 활성화 퀘스트 창 열기
    public void OpenQuestPopup()
    {
        questPopupObj.SetActive(!questPopupObj.activeSelf);
        if (questPopupObj.activeSelf)
        {
            _questPopupCtr.Showt();
        }
            
    }

    // 게임씬의 오른쪽 위 버튼 누르면 나오는 활성화 주문 창 열기
    public void OpenOrdersPopup()
    {
        orderPopupObj.SetActive(!orderPopupObj.activeSelf);
        if (orderPopupObj.activeSelf)
        {
            _orderPopupCtr.Showt();
        }
            
    }
    
    // 아직 쓸지 미정(Guest에서 구현시 삭제필요)
    public void EnableGuestOrderUI(/*GuestBase guestBase*/)
    {
        // Instantiate 말고 enabled로 바꿀수도
        var guestOrderUIObject = Instantiate(dialogPopupPrefab/* guestBase*/);
        
        guestOrderUIObject.GetComponent<OrderIconController>().Showt();
    }

    // 갯수가 필요한 팝업
    public AmountPopup CreateAmountPopup(string message, ItemData itemData, bool isPrice = false)
    {
        var popup = Instantiate(amountPopupObj, _canvas.transform).GetComponent<AmountPopup>();
        popup.Init(message, itemData, isPrice);

        return popup;
    }
}