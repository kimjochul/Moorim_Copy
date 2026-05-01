using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using static Constants;

public class CookingController : MonoBehaviour
{
    public static CookingController Instance { get; private set; }
    public bool IsCookingUIOpen => mainCookingUICanvas.activeSelf;

    [Header("UI 컴포넌트")]
    [SerializeField] private RecipeList masterRecipeList;
    [SerializeField] private GameObject mainCookingUICanvas;
    [SerializeField] private RecipeListView recipeListView;
    [SerializeField] private CookingView cookingView;
    [SerializeField] private CookingIngredientSlotUI IngredientSlotUI;

    // 내가 기억해야 할 데이터
    private CookingModel model = new CookingModel();
    private List<Recipe> runtimeRecipes = new List<Recipe>();
    private CookingStation currentStation; // 어떤 요리대인지 기억

    // 재료 반환용 인벤토리 참조
    private Inventory targetInventoryMeat; // 육류
    private Inventory targetInventoryOther; // 육류 외
    
    // UI 위치 초기값
    private Vector2 cookingInitPos;
    private Vector2 recipeInitPos;
    private Vector2 commonInvenInitPos;
    
    private RecipeUnlockManager recipeUnlockManager;
    private IngredientUnlockManager ingredientUnlockManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        mainCookingUICanvas.SetActive(false);

        recipeUnlockManager = RecipeUnlockManager.Instance;
        ingredientUnlockManager = IngredientUnlockManager.Instance;
        
        InitView();
        InitRuntimeRecipes();
        
        cookingInitPos = cookingView.GetComponent<RectTransform>().anchoredPosition;
        recipeInitPos = recipeListView.GetComponent<RectTransform>().anchoredPosition;
    }

    void OnDestroy()
    {
        if (recipeListView != null)
        {
            recipeListView.OnRecipeClicked -= OnRecipeSelected;
            recipeListView.OnCategorySelected -= ShowRecipesByCategory;
        }
        if (cookingView != null)
        {
            cookingView.OnCookClicked -= OnCookButtonClicked;
            cookingView.OnCloseClicked -= HandleClose;
        }
    }

    private void InitView()
    {
        // View 초기화
        cookingView.Init();
        recipeListView.Init();

        // View들의 방송을 구독
        recipeListView.OnRecipeClicked += OnRecipeSelected;
        recipeListView.OnCategorySelected += ShowRecipesByCategory;
        cookingView.OnCookClicked += OnCookButtonClicked;
        cookingView.OnCloseClicked += HandleClose;
    }
    
    private void ResetUIPosition()
    {
        cookingView.GetComponent<RectTransform>().anchoredPosition = cookingInitPos;
        recipeListView.GetComponent<RectTransform>().anchoredPosition = recipeInitPos;
        
    }

    private void InitRuntimeRecipes()
    {
        // 레시피리스트를 읽어서 게임용 리스트로 포장
        runtimeRecipes.Clear();
        foreach (RecipeData data in masterRecipeList.recipes)
        {
            if(recipeUnlockManager.IsUnlocked(data.id))
                runtimeRecipes.Add(new Recipe(data));
        }

        // 포장된 리스트를 메뉴 뷰에게 건네주기
        recipeListView.PopulateList(runtimeRecipes);
    }
    
    // 시스템 on/off
    public void OpenCookingSystem(CookingStation station)
    {
        currentStation = station;
        
        ResetUIPosition();
        cookingView.GetComponent<RectTransform>().DOKill(true);
        recipeListView.GetComponent<RectTransform>().DOKill(true);
        
        mainCookingUICanvas.SetActive(true);
        
        InitRuntimeRecipes();
        
        cookingView.SetCookStartInteractable(false);

        // 요리 UI 연출
        RectTransform cookingRect = this.cookingView.GetComponent<RectTransform>();
        RectTransform recipeRect = this.recipeListView.GetComponent<RectTransform>();

        UIAnimationManager.Instance.ShowGroupSlide(cookingRect, recipeRect, cookingInitPos, recipeInitPos,Direction.Right);

        // 인벤토리 애니메이션 연출

        InventoryManager.Instance.OpenCommonInventory();

        RectTransform invenRect = InventoryManager.Instance.GetCommonInventoryRect();
        commonInvenInitPos = InventoryManager.Instance.CommonInvenInitPos;
        
        if (invenRect != null)
        {
            UIAnimationManager.Instance.ShowSlide(invenRect,commonInvenInitPos, Direction.Left, 800f, 0.4f);
        }
    }

    public void CloseCookingSystem(bool returnIngredients = true)
    {
        if (returnIngredients)
            ReturnIngredientsToInventory();
        
        cookingView.ClearView();
        model.ClearTarget();

        // 요리 UI 연출
        RectTransform cookingRect = this.cookingView.GetComponent<RectTransform>();
        RectTransform recipeRect = this.recipeListView.GetComponent<RectTransform>();

        UIAnimationManager.Instance.HideGroupSlide(cookingRect, recipeRect, cookingInitPos, recipeInitPos, Direction.Right,
        onComplete: () => {
            mainCookingUICanvas.SetActive(false);
        });

        // 인벤토리 애니메이션 연출
        RectTransform invenRect = InventoryManager.Instance.GetCommonInventoryRect();
        commonInvenInitPos = InventoryManager.Instance.CommonInvenInitPos;

        if (invenRect != null)
        {
            UIAnimationManager.Instance.HideSlide(invenRect, commonInvenInitPos, Direction.Left, 800f, 0.3f,
                onComplete: () =>
                {
                    InventoryManager.Instance.CloseCommonInventory();
                }
            );
        }

        currentStation = null;
        targetInventoryMeat = null;
        targetInventoryOther = null;
    }

    private void HandleClose()
    {
        CloseCookingSystem();
    }

    // 요리 시작 안 하고 창을 닫았을 때 인벤토리로 다시 재료를 돌려주는 메소드 
    private void ReturnIngredientsToInventory()
    {
        if (targetInventoryMeat == null && targetInventoryOther == null) 
            return;
        
        foreach (var kvp in model.InsertedIngredients)
        {
            int ingredientId = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0)
                continue;

            if (!model.TryGetCachedItem(ingredientId, out IItem returnItem))
                continue;

            if (returnItem.Data is IngredientData ingredientData)
            {
                Inventory targetIven = (ingredientData.type == IngredientType.Meats) ? targetInventoryMeat : targetInventoryOther;

                targetIven?.GetItem(returnItem, amount);
            }

            Debug.Log($"[재료 반환] ID: {ingredientId}, {amount}개를 인벤토리로 반환");
        } 
    }

    // 메뉴에서 요리가 선택 됐을 때 함수
    // 매개 변수 : 선택된 레시피
    // 1. 모델에게 타겟 설정하라 지시.
    // 2. 조리 뷰에게 화면 바꾸라 지시/
    // 3. 조리 버튼 잠금
    public void OnRecipeSelected(Recipe selectedRecipe)
    {
        ReturnIngredientsToInventory();
        
        // 모델에게 타겟 설정, 조리뷰에게 화면 바꾸기 지시, 조리 버튼 잠금
        model.SetTargetRecipe(selectedRecipe);
        cookingView.UpdateRecipe(selectedRecipe);
        cookingView.SetCookStartInteractable(false);
    }

    // 인벤토리에서 재료가 드랍됐을 때
    // 매개 변수 : 드랍된 재료/갯수
    // 1. 모델에게 투입 검사 지시 -> 통과 시 ui 숫자 올림
    // 2. 재료 다 모이면 조리 버튼 활성화
    public void OnIngredientClicked(Slot inventorySlot)
    {
        if (inventorySlot.IsEmpty || inventorySlot.Item == null)
            return;

        // 인벤토리 출력 기억
        CacheInventorySource(inventorySlot);

        // IItem 인터페이스에서 ID 가져오기
        int ingredientId = inventorySlot.Item.ID;

        // 원본 아이템 캐싱
        model.CacheItem(ingredientId, inventorySlot.Item);
        
        // 투입시도
        // 클릭 한 번에 재료 하나씩 투입하는 방식
        int consumedAmount = model.TryAddIngredient(ingredientId, 1);

        if (consumedAmount > 0)
            ApplyIngredientConsumption(inventorySlot, ingredientId, consumedAmount);
        else
            Debug.Log("이 요리에 필요 없거나, 이미 꽉 찬 재료입니다!");
    }

    private void CacheInventorySource(Slot slot)
    {
        if (slot.Owner.Type == InventoryType.IngredientsButMeatOnly)
            targetInventoryMeat = slot.Owner;
        else if (slot.Owner.Type == InventoryType.Ingredients)
            targetInventoryOther = slot.Owner;
    }

    private void ApplyIngredientConsumption(Slot slot, int ingredientId, int consumed)
    {
        // 실제로 들어간 갯수 만큼 조리뷰 재료 슬롯에 숫자 올리기
        cookingView.UpdateSlotUI(ingredientId, consumed);
 
        /// Slot 클래스의 AddCount와 Clear를 그대로 사용해서 아이템 차감
        slot.AddCount(-consumed);
        if (slot.Count <= 0)
            slot.Clear();
 
        // 재료가 다 모이면 버튼 활성화
        if (model.IsReadyToCook())
            cookingView.SetCookStartInteractable(true);
    }

    // 조리 시작 버튼 눌렀을 때
    // 스테이션에게 요리 시작하라 명령 시스템 끄기
    public void OnCookButtonClicked()
    {
        if (currentStation != null && model.IsReadyToCook())
        {
            currentStation.StartCooking(model.CurrentRecipe);
            model.InsertedIngredients.Clear();
            CloseCookingSystem(false);
        }
    }

    // 카테고리 필터
    public void ShowRecipesByCategory(Constants.FoodType targetCategory)
    {
        List<Recipe> fillterList = runtimeRecipes.Where(r => r.Data.foodType == targetCategory).ToList();

        recipeListView.PopulateList(fillterList);
    }
}
