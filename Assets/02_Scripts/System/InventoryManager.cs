using System;
using UnityEngine;


/// <summary>
/// 인벤토리 UI를 조종할 때가 있을 때 사용하기 위한 관리 매니저
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    // 미리 생성되어진 인벤토리들 관련 매니저
    // 인벤토리 종류가 5개 이니까 관리하는 클래스가 될 듯
    // 각 인벤토리들의 UI 패널들은 무조건 InventoryPopUpController 있어야함
    
    [SerializeField] private InventoryPopupController commonInventoryRoot; // 공용 인벤토리 루트의 위치 공용 인벤토리 켜질 때 사용 
    
    [SerializeField] private Inventory meatIngredientInventory; // 고기 재료용 인벤토리
    [SerializeField] private Inventory otherIngredientInventory; // 다른 재료용 인벤토리

    //[SerializeField] private Inventory bondItemInventory; // 인연 아이템

    [SerializeField] private Inventory chefInventory; // 소월이용 인벤토리
    //[SerializeField] private Inventory serverInventory; // 명월이용 인벤토리

    //[SerializeField] private Inventory refridgeInventory; // 냉장고
    //[SerializeField] private Inventory mealTableInventory; // 배식대

    private void Start()
    {
        OpenCommonInventory();
        OpenInventory(InventoryType.Chef);
    }

    public Inventory GetInventory(InventoryType type)
    {
        return type switch
        {
            InventoryType.IngredientsButMeatOnly => meatIngredientInventory,
            InventoryType.Ingredients => otherIngredientInventory,
            //InventoryType.BondItem => bondItemInventory,
            InventoryType.Chef => chefInventory,
            //InventoryType.Server => serverInventory,
            //InventoryType.Fridge => refridgeInventory,
            //InventoryType.MealTable => mealTableInventory,
            _ => null
        };
    }

    public void OpenCommonInventory()
    {
        UIManager.Instance.OpenCommonInventoryPopup(commonInventoryRoot);
    }

    public void CloseCommonInventory()
    {
        UIManager.Instance.CloseCommonInventoryPopup(commonInventoryRoot);
    }
    
    public void OpenInventory(InventoryType type)
    {
        var inventory = GetInventory(type);

        if (inventory == null)
        {
            Debug.LogError("Inventory 없음");
            return;
        }

        UIManager.Instance.OpenInventoryPopup(inventory);
    }

    public void CloseInventory(InventoryType type)
    {
        var inventory = GetInventory(type);

        if (inventory == null)
        {
            Debug.LogError($"{type} 인벤토리 없음");
            return;
        }

        UIManager.Instance.CloseInventoryPopup(inventory);
    }
}
