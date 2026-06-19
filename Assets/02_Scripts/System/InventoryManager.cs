using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;
using UnityEngine.SceneManagement;


/// <summary>
/// 인벤토리 UI를 조종할 때가 있을 때 사용하기 위한 관리 매니저
/// </summary>
public class InventoryManager : Singleton<InventoryManager>, IInitializable
{
    // 미리 생성되어진 인벤토리들 관련 매니저
    // 인벤토리 종류가 5개 이니까 관리하는 클래스가 될 듯
    // 각 인벤토리들의 UI 패널들은 무조건 InventoryPopUpController 있어야함
    
    [SerializeField] private InventoryPopupController commonInventoryRoot; // 공용 인벤토리 루트의 위치 공용 인벤토리 켜질 때 사용 
    
    [SerializeField] private Inventory meatIngredientInventory; // 고기 재료용 인벤토리
    [SerializeField] private Inventory otherIngredientInventory; // 다른 재료용 인벤토리

    [SerializeField] private Inventory bondItemInventory; // 인연 아이템

    [SerializeField] private Inventory chefInventory; // 소월이용 인벤토리
    [SerializeField] private Inventory serverInventory; // 명월이용 인벤토리

    [SerializeField] private Inventory fridgeInventory; // 냉장고
    [SerializeField] private Inventory mealTableInventory; // 배식대

    private Dictionary<InventoryType, Inventory> inventoryMap;

    public Vector2 CommonInvenInitPos;
    
    public event Action<Inventory> OnInventoryUIOpened;
    public event Action<Inventory> OnInventoryUIClosed;
    public event Action<bool> OnBottomUIToggleRequested;
    
    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    protected override void Awake()
    {
        base.Awake();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Init()
    {
        if (isInitialized) return;
        
        // 🔥 씬에 있는 Inventory 전부 찾아서 Init
        var inventories = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        SetCommonInventoryRoot();
        
        
        if (inventories.Length == 0)
        {
            Debug.LogError("Inventory 없음 Init 실패");
            return;
        }

        inventoryMap = new Dictionary<InventoryType, Inventory>();
        
        foreach (var inv in inventories)
        {
            Debug.Log(inv.Type.ToString());
            Register(inv);
            inv.Init();
        }
        isInitialized = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var inventories = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        SetCommonInventoryRoot();
        
        if (inventories.Length == 0)
        {
            Debug.LogError("Inventory 없음 Init 실패");
            return;
        }

        inventoryMap = new Dictionary<InventoryType, Inventory>();
        
        foreach (var inv in inventories)
        {
            Debug.Log(inv.Type.ToString());
            Register(inv);
            inv.Init();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public Inventory GetInventory(InventoryType type)
    {
        if (!inventoryMap.ContainsKey(type))
        {
            Debug.LogError($"[InventoryManager] {type} 아직 등록 안됨");
        }
        return inventoryMap.GetValueOrDefault(type);
    }

    /// <summary>
    /// 공용 인벤토리 열기
    /// </summary>
    public void OpenCommonInventory()
    {
        UIManager.Instance.OpenCommonInventoryPopup(commonInventoryRoot);
    }

    /// <summary>
    /// 공용 인벤토리 닫기
    /// </summary>
    public void CloseCommonInventory()
    {
        UIManager.Instance.CloseCommonInventoryPopup(commonInventoryRoot);
    }

    /// <summary>
    /// 공용 인벤토리의 RectTransform 반환
    /// </summary>
    public RectTransform GetCommonInventoryRect()
    {
        if (this.commonInventoryRoot != null)
        {
            return this.commonInventoryRoot.GetComponent<RectTransform>();
        }
        return null;
    }

    /// <summary>
    /// 특정 인벤토리 열기
    /// </summary>
    /// <param name="type"></param>
    public void OpenInventory(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError($"{inventory.Type.ToString()} Inventory 없음");
            return;
        }

        Debug.Log($"{inventory.Type} 열기 시도");
        UIManager.Instance.OpenInventoryPopup(inventory);
        OnInventoryUIOpened?.Invoke(inventory);
    }

    /// <summary>
    /// 특정 인벤토리 닫기
    /// </summary>
    /// <param name="type"></param>
    public void CloseInventory(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError($"인벤토리 없음");
            return;
        }
        UIManager.Instance.CloseInventoryPopup(inventory);
        OnInventoryUIClosed?.Invoke(inventory);
    }

    public void RequestBottomUI(bool open)
    {
        OnBottomUIToggleRequested?.Invoke(open);
    }
    
    public void Register(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("Register: inventory 자체 null");
            return;
        }
        
        Debug.Log($"[Register] {inventory.name} / type: {inventory.Type}");
    
        if (inventoryMap.ContainsKey(inventory.Type))
        {
            Debug.LogWarning($"중복 타입 발생: {inventory.Type}");
        }
        
        inventoryMap[inventory.Type] = inventory;
        
        // 🔥 디버그용: 인스펙터 필드에도 직접 연결
        switch (inventory.Type)
        {
            case InventoryType.MealTable:
                mealTableInventory = inventory;
                break;

            case InventoryType.IngredientsButMeatOnly:
                meatIngredientInventory = inventory;
                break;

            case InventoryType.Server:
                serverInventory = inventory;
                break;

            case InventoryType.Chef:
                chefInventory = inventory;
                break;

            case InventoryType.Ingredients:
                otherIngredientInventory = inventory;
                break;

            case InventoryType.Fridge:
                fridgeInventory = inventory;
                break;
            
            case InventoryType.BondItem:
                bondItemInventory = inventory;
                break;

            default:
                Debug.LogWarning($"[Register] 매핑 안된 타입: {inventory.Type}");
                break;
        }
    }
    
    public List<Inventory> GetAll()
    {
        return inventoryMap.Values.ToList();
    }
    
    public void ResetData()
    {
        foreach (var inv in GetAll())
        {
            if (inv == null)
                continue;

            Debug.Log($"[ResetData] {inv.Type}");
            
            inv.ResetData();
        }

        CreateInitItems();
    }
    
    public void SetCommonInventoryRoot()
    {
        var popups = FindObjectsByType<InventoryPopupController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        InventoryPopupController found = null;

        foreach (var popup in popups)
        {
            if (!popup.IsCommonInventory())
                continue;

            if (found != null)
            {
                Debug.LogError("CommonInventoryRoot 2개 이상 있음");
            }

            found = popup;
        }

        if (found == null)
        {
            Debug.LogError("CommonInventoryRoot 못 찾음");
        }

        commonInventoryRoot = found;
        CommonInvenInitPos = commonInventoryRoot.GetComponent<RectTransform>().anchoredPosition;
    }
    
    // ResetData의 CreateInitItems도 같이 끔.
    private void CreateInitItems()
    {
        List<Constants.IngredientType> ingredientTypes = new List<Constants.IngredientType>();
        ingredientTypes.Add(Constants.IngredientType.Meats);
        ingredientTypes.Add(Constants.IngredientType.Vegetable);
        ingredientTypes.Add(Constants.IngredientType.Processed);
        ingredientTypes.Add(Constants.IngredientType.Spices);
        foreach(var a in ingredientTypes)
        {
            var ingredientList = DataManager.Instance.GetIngredientList(a);
            foreach (var ingredientData in ingredientList)
            {
                Item item = ItemFactory.Instance.CreateItem(ingredientData.id);

                if (ingredientData.type == Constants.IngredientType.Meats)
                {
                    InventoryManager.Instance.GetInventory(InventoryType.IngredientsButMeatOnly).GetItem(item, 10);

                }
                else
                    InventoryManager.Instance.GetInventory(InventoryType.Ingredients).GetItem(item, 20);
            }
        }
    }
    
    public void UpgradeInventorySlot(InventoryType type, int amount)
    {
        Inventory targetInventory = GetInventory(type);
        
        if (targetInventory != null)
        {
            targetInventory.AddSlot(amount);
            Debug.Log($"[Upgrade] {type} 인벤토리 슬롯이 {amount}개 확장. (현재 총 {targetInventory.Slots.Count}개)");
        }
        else
        {
            Debug.Log($"[Upgrade] {type} 인벤토리를 찾을 수 없어 확장에 실패.");
        }
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
            UpgradeInventorySlot(InventoryType.Server, 1);
        if (Keyboard.current.nKey.wasPressedThisFrame)
            UpgradeInventorySlot(InventoryType.Chef, 1);
        if (Keyboard.current.mKey.wasPressedThisFrame)
            UpgradeInventorySlot(InventoryType.MealTable, 1);
        if (Keyboard.current.vKey.wasPressedThisFrame)
            UpgradeInventorySlot(InventoryType.Fridge, 1);

    }
}
