using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

/// <summary>
/// 상인과 인벤토리, 객잔의 economymanager를 연결해서 아이템 구매하는데 사용할 클래스
/// </summary>
public class Shop : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Inventory playerMeatInventory;
    [SerializeField] private Inventory playerOtherIngredientInventory;
    [SerializeField] private Inventory fridgeInventory;

    [SerializeField] private GameObject objCategory;
    [SerializeField] private Transform tsCategory;
    [SerializeField] private GameObject objProduct;
    [SerializeField] private Transform tsProduct;
    [SerializeField] private Button btnClose;

    // 카테고리
    private Dictionary<IngredientType, ShopCategory> dicCategory = new Dictionary<IngredientType, ShopCategory>();
    [SerializeField]
    private ToggleGroup categoryGroup;

    // 상점 요소
    private List<GameObject> listProduct = new List<GameObject>();

    public Action<ItemData> OnProductClicked;

    [SerializeField]
    private ShopProductInfo infoPanel;

    //void Start()
    //{
    //    Init();
    //}
    public void Init()
    {
        playerMeatInventory = InventoryManager.Instance.GetInventory(InventoryType.IngredientsButMeatOnly);
        playerOtherIngredientInventory = InventoryManager.Instance.GetInventory(InventoryType.Ingredients);
        fridgeInventory = InventoryManager.Instance.GetInventory(InventoryType.Fridge);

        Buy(0, 1);
        Buy(5, 2);
        Buy(6, 1);

        CreateCategory();
    }

    public bool Buy(int itemID, int count)
    {
        Debug.Log("Buying " + itemID);
        // 돈 체크는 생략
        Item item = itemFactory.CreateItem(itemID);

        int totalPrice = item.Data.price * count;
        
        if (!EconomyManager.Instance.CanSpend(totalPrice))
        {
            Debug.Log("돈 부족");
            return false;
        }
        
        EconomyManager.Instance.SpendMoney(totalPrice);
        
        if (item.Data is IngredientData ingredientData)
        {
            if (ingredientData.type == Constants.IngredientType.Meats)
            {
                //Debug.Log($"{ingredientData.name} is a meat");
                playerMeatInventory.GetItem(item, count);
            }
            else
            {
                //Debug.Log($"{ingredientData.name} is not a meat");
                playerOtherIngredientInventory.GetItem(item, count);
            }
        }
        else if (item.Data is FoodData foodData)
        {
            fridgeInventory.GetItem(item, count);
            Debug.Log($"{foodData.name} is a food");
        }
        else if (item.Data is BondItemData bondItemData)
        {
            Debug.Log($"{bondItemData.name} is a bondItem");
        }
        else
        {
            Debug.Log("Not a valid item");
        }

        return true;
    }
    // 카테고리 생성
    public void CreateCategory()
    {
        int length = System.Enum.GetValues(typeof(IngredientType)).Length;
        for (int i = 0; i < length; i++)
        {
            var type = (IngredientType)i;
            string s = (type).ToKorean().ToString();

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
    // 상점 요소 생성
    public void CreateProduct()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject slot = Instantiate(this.objProduct, this.tsProduct);

            slot.SetActive(false);

            this.listProduct.Add(slot);
        }
    }

    // 카테고리 클릭시 카테고리에 해당되는 상점 요소로 바꾸기
    private void UpdateProductList(IngredientType type)
    {
        List<IngredientData> targetDataList = DataManager.Instance.GetIngredientList(type);

        int minSlotCount = 3;
        int displayCount = Mathf.Max(targetDataList.Count, minSlotCount);

        // 빈 슬롯이 부족하면 생성
        if (this.listProduct.Count < displayCount)
        {
            int needCount = displayCount - this.listProduct.Count;
            for (int i = 0; i < needCount; i++)
            {
                GameObject product = Instantiate(this.objProduct, this.tsProduct);
                product.SetActive(false);
                this.listProduct.Add(product);
            }
        }

        // 상점 요소 띄우기
        for (int i = 0; i < this.listProduct.Count; i++)
        {
            this.listProduct[i].SetActive(false);

            ShopProduct product = this.listProduct[i].GetComponent<ShopProduct>();
            product.Clear();
        }

        for (int i = 0; i < displayCount; i++)
        {
            this.listProduct[i].SetActive(true);
            ShopProduct product = this.listProduct[i].GetComponent<ShopProduct>();

            // 데이터가 있는 정상 슬롯일 경우
            if (i < targetDataList.Count)
            {
                product.Init(targetDataList[i]);

                product.onClickBuy += (itemData) =>
                {
                    OnProductClicked?.Invoke(itemData);
                };

                product.onHoverEnter += (itemData) => {
                    this.infoPanel.ShowInfo(itemData);
                };

                product.onHoverExit += () => {
                    this.infoPanel.HideInfo();
                };
            }
        }
    }
    
}
