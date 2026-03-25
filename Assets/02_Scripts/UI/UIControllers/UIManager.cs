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
    [SerializeField] private Button _questButton;
    [SerializeField] private Button _orderButton;
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
    
    public void OpenDialogPopup(string characterName, string dialog)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(characterName, dialog);
    }
    
    public void OpenQuestPopup()
    {
        questPopupObj.SetActive(!questPopupObj.activeSelf);
        if (questPopupObj.activeSelf)
        {
            questPopupObj.GetComponent<QuestPopupController>().Show(/*QuestManager.Instance.activeQuests*/);
        }
            
    }

    public void OpenOrdersPopup()
    {
        orderPopupObj.SetActive(!orderPopupObj.activeSelf);
        if (orderPopupObj.activeSelf)
        {
            orderPopupObj.GetComponent<OrdersPopupController>().Show(tempOrders);
        }
            
    }
    
    public void EnableGuestOrderUI(/*GuestBase guestBase*/)
    {
        // Instantiate 말고 enabled로 바꿀수도
        var guestOrderUIObject = Instantiate(dialogPopupPrefab/* guestBase*/);
        
        guestOrderUIObject.GetComponent<OrderIconController>().Showt();
    }
}