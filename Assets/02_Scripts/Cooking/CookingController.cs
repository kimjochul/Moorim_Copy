using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingController : MonoBehaviour
{
    public static CookingController Instance { get; private set; }
    public bool IsCookingUIOpne => mainCookingUICanvas.activeSelf;

    [Header("UI 컴포넌트")]
    [SerializeField] private RecipeList masterRecipeList;
    [SerializeField] private GameObject mainCookingUICanvas;
    [SerializeField] private RecipeListView recipeListView;
    [SerializeField] private CookingView cookingview;
    [SerializeField] private CookingIngredientSlotUI IngredientSlotUI;

    // 내가 기억해야 할 데이터
    private CookingModel model = new CookingModel();
    private List<Recipe> runtimeRecipes = new List<Recipe>();
    private CookingStation currentStation; // 어떤 요리대인지 기억
    private Inventory targetInventory; // 아이템 돌려줄 때 어느 인벤토리에서 온건지 기억

    private Dictionary<int, IItem> cachedItems = new Dictionary<int, IItem>(); // ID(int)를 넣으면 원본 아이템(IItem)을 뱉어내는 임시 기억 장치

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCookingUICanvas.SetActive(false);
        cookingview.Init(this);
        recipeListView.Init(this);

        // 레시피리스트를 읽어서 게임용 리스트로 포장
        runtimeRecipes.Clear();
        foreach (RecipeData data in masterRecipeList.recipes)
            runtimeRecipes.Add(new Recipe(data));

        // 포장된 리스트를 메뉴 뷰에게 건네주기
        recipeListView.PopulateList(runtimeRecipes);
    }

    // 메소드2. 시스템 on/off
    public void OpenCookingSystem(CookingStation station)
    {
        currentStation = station;
        mainCookingUICanvas.SetActive(true);
        UIManager.Instance.OpenCommonInventoryPopup();
    }

    public void CloseCookingSystem()
    {
        ReturnIngredientsToInventory();
        
        cookingview.ClearView();
        model.ClearTarget();

        mainCookingUICanvas.SetActive(false);
        UIManager.Instance.DestroyCommonInventoryPopup();
        currentStation = null;
    }

    // 요리 시작 안 하고 창을 닫았을 때 인벤토리로 다시 재료를 돌려주는 메소드 
    private void ReturnIngredientsToInventory()
    {
        if (targetInventory == null) 
            return;
        
        foreach (var kvp in model.InsertedIngredients)
        {
            int ingredientId = kvp.Key;
            int amount = kvp.Value;

            if (amount > 0)
            {
                IItem returnItem = cachedItems[ingredientId];

                targetInventory.GetItem(returnItem, amount);
                Debug.Log($"[재료 반환] 취소됨! ID: {ingredientId} 재료 {amount}개를 인벤토리로 다시 돌려보냅니다.");
            }
        }
    }

    // 메소드3. 메뉴에서 요리가 선택 됐을 때 함수
    // 매개 변수 : 선택된 레시피
    // 1. 모델에게 타겟 설정하라 지시.
    // 2. 조리 뷰에게 화면 바꾸라 지시/
    // 3. 조리 버튼 잠금
    public void OnRecipeSelected(Recipe selectedRecipe)
    {
        // 모델에게 타겟 설정, 조리뷰에게 화면 바꾸기 지시, 조리 버튼 잠금
        model.SetTargetRecipe(selectedRecipe);
        cookingview.UpdateRecipe(selectedRecipe);
        cookingview.SetCookStartInteractable(false);
    }

    // 메소드4. 인벤토리에서 재료가 드랍됐을 때
    // 매개 변수 : 드랍된 재료/갯수
    // 1. 모델에게 투입 검사 지시 -> 통과 시 ui 숫자 올림
    // 2. 재료 다 모이면 조리 버튼 활성화
    public void OnIngredientClicked(Slot inventorySlot)
    {
        if (inventorySlot.IsEmpty || inventorySlot.Item == null)
            return;

        targetInventory = inventorySlot.Owner;
        // IItem 인터페이스에서 ID 가져오기
        int ingredientId = inventorySlot.Item.ID;

        // 나중에 돌려줄 상황을 대비해서, 원본 아이템 객체를 기억해둡니다!
        if (!cachedItems.ContainsKey(ingredientId))
            cachedItems.Add(ingredientId, inventorySlot.Item);
        
        // 모델에게 투입 검사 지시
        // 클릭 한 번에 재료 하나씩 투입하는 방식
        int consumedAmount = model.TryAddIngredient(ingredientId, 1);

        if (consumedAmount > 0)
        {
            // 실제로 들어간 갯수 만큼 조리뷰 재료 슬롯에 숫자 올리기
            cookingview.UpdateSlotUI(ingredientId, consumedAmount);

            // Slot 클래스의 AddCount와 Clear를 그대로 사용해서 아이템 차감
            inventorySlot.AddCount(-consumedAmount);
            if (inventorySlot.Count <= 0)
                inventorySlot.Clear();
            
            // 재료가 다 모이면 버튼 활성화
            if (model.IsReadyToCook())
                cookingview.SetCookStartInteractable(true);
        }
        else
        {
            // 0이 반환되었다면
            Debug.Log("이 요리에 필요 없거나, 이미 꽉찬 재료입니다!");
        }
    }

    // 메소트5. 조리 시작 버튼 눌렀을 때
    // 스테이션에게 요리 시작하라 명령 시스템 끄기
    public void OnCookButtonClicked()
    {
        if (currentStation != null)
        {
            currentStation.StartCooking(model.CurrentRecipe);
            model.InsertedIngredients.Clear();
            CloseCookingSystem();
        }
    }

    public void ShowRecipesByCategory(Constants.FoodType targetCategory)
    {
        List<Recipe> fillterList = runtimeRecipes.Where(r => r.Data.foodType == targetCategory).ToList();

        recipeListView.PopulateList(fillterList);
    }
}
