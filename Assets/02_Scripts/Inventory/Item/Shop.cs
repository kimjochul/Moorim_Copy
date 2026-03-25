using System;
using UnityEngine;

/// <summary>
/// 상인과 인벤토리, 객잔의 economymanager를 연결해서 아이템 구매하는데 사용할 클래스
/// </summary>
public class Shop : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Inventory playerMeatInventory;
    [SerializeField] private Inventory playerOtherIngredientInventory;
    [SerializeField] private Inventory playerFoodInventory;

    private void Start()
    {
        // Buy(0,3);
        // Buy(11, 5);
        // Buy(101,3);
    }

    public void Buy(int itemID, int count)
    {
        Debug.Log("Buying " + itemID);
        // 돈 체크는 생략
        Item item = itemFactory.CreateItem(itemID);

        if (item.Data is IngredientData ingredientData)
        {
            if (ingredientData.type == Constants.IngredientType.Meats)
            {
                Debug.Log($"{ingredientData.name} is a meat");
                playerMeatInventory.GetItem(item, count);
            }
            else
            {
                Debug.Log($"{ingredientData.name} is not a meat");
                playerOtherIngredientInventory.GetItem(item, count);
            }
        }
        else if (item.Data is FoodData foodData)
        {
            Debug.Log($"{foodData.name} is a food");
            playerFoodInventory.GetItem(item, count);
        }
        else if (item.Data is BondItemData bondItemData)
        {
            Debug.Log($"{bondItemData.name} is a bondItem");
        }
        else
        {
            Debug.Log("Not a valid item");
        }
    }
}
