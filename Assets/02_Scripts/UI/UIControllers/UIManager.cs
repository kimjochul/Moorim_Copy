using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;
using static Constants;

public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;
    
    [SerializeField] private GameObject amountPopupObj;
    [SerializeField] private GameObject okPopupObj;

    QuestPopupController _questPopupCtr;
    OrdersPopupController _orderPopupCtr;
    [SerializeField] private GameObject defaultUI;

    [SerializeField] private BookView book;
    [SerializeField] private Setting setting;
    
    private bool _isQuestPopupOpen = false;
    private bool _isOrderPopupOpen = false;

    private GameObject inventory;

    public bool IsBookActive => book.gameObject.activeSelf; // 혹은 book.IsShowing 같은 프로퍼티
    public bool IsSettingActive => setting.gameObject.activeSelf;
    
    // temp 
    private Dictionary<Recipe, int> tempRecipes;
    [SerializeField] private List<RecipeData> recipes;
    List<Order> tempOrders = new List<Order>();
    
    protected override void Awake()
    {
        base.Awake();
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        
        _questPopupCtr = FindFirstObjectByType<QuestPopupController>();
        _orderPopupCtr = FindFirstObjectByType<OrdersPopupController>();
        
        // OpenDefaultUI();
        
        
        // temp
        foreach (var recipe in recipes)
        {
            Recipe newRecipe = new Recipe(recipe);
            Dictionary<Recipe, int> order = new  Dictionary<Recipe, int>();
            order.Add(newRecipe, Random.Range(0, 3));
            tempOrders.Add(new Order(order));
        }
    }

    // public void OpenDefaultUI()
    // {
    //     Instantiate(defaultUI,  _canvas.transform);
    // }
    
    // 공용 인벤토리 호출시 나오는 함수
    public void OpenCommonInventoryPopup(InventoryPopupController commonInventory)
    {
        if (commonInventory == null) return;

        commonInventory.Show();
    }
    
    // 공용 인벤 내릴 때 호출
    public void CloseCommonInventoryPopup(InventoryPopupController commonInventory)
    {
        if (commonInventory == null) return;

        commonInventory.Hide();
    }
    
    // 각 인벤토리 열 때 호출되는 함수
    public void OpenInventoryPopup(Inventory inventory)
    {
        var popup = inventory.transform.parent.GetComponent<InventoryPopupController>();
        
        if (popup == null)
        {
            Debug.LogError("Popup 없음");
            return;
        }

        popup.Show();
    }
    
    // 공용 인벤토리 닫기
    public void CloseInventoryPopup(Inventory inventory)
    {
        var popup = inventory.GetComponentInParent<InventoryPopupController>();

        if (popup != null)
        {
            popup.Hide();
        }
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
            _orderPopupCtr.Show();
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
    public AmountPopup CreateAmountPopup(string message, ItemData itemData, UnityAction<int> onOk, UnityAction onCancel)
    {
        var popup = Instantiate(amountPopupObj, _canvas.transform).GetComponent<AmountPopup>();
        popup.Init(itemData);
        popup.ButtonOk(onOk);
        popup.ButtonCancel(onCancel);

        return popup;
    }
    // 설명을 띄워줄 팝업
    public OkPopup CreateOkPopup(string message, UnityAction onOk)
    {
        var popup = Instantiate(okPopupObj, _canvas.transform).GetComponent<OkPopup>();
        popup.Init(message);
        popup.ButtonOk(onOk);

        return popup;
    }

    // 도감
    public void ActiveBook(bool isShow)
    {
        book.ShowBook(isShow);
    }
    // 설정창
    public void ActiveSetting(bool isShow)
    {
        if (isShow)
            setting.Show(); 
        else
            setting.Hide();
    }
}