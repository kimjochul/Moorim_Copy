using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Constants;

/// <summary>
/// 상인과 인벤토리, 객잔의 economymanager를 연결해서 아이템 구매하는데 사용할 클래스
/// </summary>
public class Shop : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Inventory playerMeatInventory;
    [SerializeField] private Inventory playerOtherIngredientInventory;
    [SerializeField] private Inventory playerFoodInventory;

    [SerializeField] private GameObject objCategory;
    [SerializeField] private Transform tsCategory;
    [SerializeField] private GameObject objProduct;
    [SerializeField] private Transform tsProduct;

    // 카테고리
    private Dictionary<IngredientType, ShopCategory> dicCategory = new Dictionary<IngredientType, ShopCategory>();
    [SerializeField]
    private ToggleGroup categoryGroup;

    // 상점 요소
    private List<GameObject> listProduct = new List<GameObject>();


    private void Start()
    {
        // Buy(0,3);
        // Buy(11, 5);
        // Buy(101,3);

        CreateCategory();
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
    public void CreateCategory()
    {
        int length = System.Enum.GetValues(typeof(IngredientType)).Length;
        for (int i = 0; i < length; i++)
        {
            var type = (IngredientType)i;
            string s = (type).ToString();

            ShopCategory category = Instantiate(this.objCategory, this.tsCategory).GetComponent<ShopCategory>();
            category.Init(s);
            category.SetToggleGloup(this.categoryGroup);

            category.onToggleEvent = (bool isOn) =>
            {
                if (isOn)
                {
                    UpdateProductList(type);
                }
            };

            this.dicCategory.Add(type, category);
        }
    }
    public void CreateProduct()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject slot = Instantiate(this.objProduct, this.tsProduct);

            slot.SetActive(false);

            this.listProduct.Add(slot);
        }
    }
    private void UpdateProductList(IngredientType type)
    {
        List<IngredientData> targetDataList = DataManager.Instance.GetIngredientList(type);

        if (this.listProduct.Count < targetDataList.Count)
        {
            int needCount = targetDataList.Count - this.listProduct.Count;
            for (int i = 0; i < needCount; i++)
            {
                GameObject product = Instantiate(this.objProduct, this.tsProduct);
                product.SetActive(false);
                this.listProduct.Add(product);
            }
        }

        for (int i = 0; i < this.listProduct.Count; i++)
        {
            if (i < targetDataList.Count)
            {
                this.listProduct[i].SetActive(true);

                ShopProduct slotScript = this.listProduct[i].GetComponent<ShopProduct>();

                slotScript.Init(targetDataList[i]);
            }
            else
            {
                this.listProduct[i].SetActive(false);
            }
        }
    }
}
