using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;
    [SerializeField] private GameObject commonInventoryPopupPrefab;
    
    [SerializeField] private GameObject amountPopupObj;
    [SerializeField] private GameObject inventoryObj;

    
    
    QuestPopupController _questPopupCtr;
    OrdersPopupController _orderPopupCtr;
    [SerializeField] private GameObject defaultUI;
    
    private bool _isQuestPopupOpen = false;
    private bool _isOrderPopupOpen = false;

    private GameObject inventory;
    
    // temp 
    private Dictionary<Recipe, int> tempRecipes;
    [SerializeField] private GuestData tempGuestData;
    [SerializeField] private List<RecipeData> recipes;
    List<Order> tempOrders = new List<Order>();
    
    protected override void Awake()
    {
        base.Awake();
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        
        _questPopupCtr = FindFirstObjectByType<QuestPopupController>();
        _orderPopupCtr = FindFirstObjectByType<OrdersPopupController>();
        
        OpenDefaultUI();
        
        
        // temp
        foreach (var recipe in recipes)
        {
            Recipe newRecipe = new Recipe(recipe);
            Dictionary<Recipe, int> order = new  Dictionary<Recipe, int>();
            order.Add(newRecipe, Random.Range(0, 3));
            tempOrders.Add(new Order(order, tempGuestData));
        }
    }

    public void OpenDefaultUI()
    {
        Instantiate(defaultUI,  _canvas.transform);
    }
    
    // 공용 인벤토리 열기
    public void OpenCommonInventoryPopup(bool open = true)
    {
        var inven = inventoryObj.GetComponent<InventoryPopupController>();
        inventoryObj.SetActive(open);
        if (inven == null)
        {
            return;
        }
        inven.Show();
    }
    
    // 공용 인벤토리 닫기
    public void DestroyCommonInventoryPopup()
    {
        inventoryObj.SetActive(false);
    }
    
    // 스토리에서 대화창 열기
    public void OpenDialogPopup(string characterName, string dialog)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(characterName, dialog);
    }
    
    // 게임씬의 오른쪽 위 버튼 누르면 나오는 활성화 퀘스트 창 열기
    public void OpenQuestPopup(GameObject questObj)
    {
        questObj.SetActive(!questObj.activeSelf);
        if (questObj.activeSelf)
        {
            _questPopupCtr = defaultUI.GetComponent<QuestPopupController>();
            if (_questPopupCtr == null)
            {
                Debug.LogError("QuestPopupCtrl is null");
                return;
            }
            _questPopupCtr.Showt();
        }
            
    }

    // 게임씬의 오른쪽 위 버튼 누르면 나오는 활성화 주문 창 열기
    public void OpenOrdersPopup(GameObject orderObj)
    {
        orderObj.SetActive(!orderObj.activeSelf);
        if (orderObj.activeSelf)
        {
            _orderPopupCtr = defaultUI.GetComponent<OrdersPopupController>();
            if (_orderPopupCtr == null)
            {
                Debug.LogError("OrdersPopupCtrl is null");
                return;
            }
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