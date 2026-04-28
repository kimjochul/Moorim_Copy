using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Constants;


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

    //[SerializeField] private Inventory bondItemInventory; // 인연 아이템

    [SerializeField] private Inventory chefInventory; // 소월이용 인벤토리
    [SerializeField] private Inventory serverInventory; // 명월이용 인벤토리

    [SerializeField] private Inventory fridgeInventory; // 냉장고
    [SerializeField] private Inventory mealTableInventory; // 배식대

    private Dictionary<InventoryType, Inventory> inventoryMap;
    
    public event Action<Inventory> OnInventoryUIOpened;
    public event Action<Inventory> OnInventoryUIClosed;
    public event Action<bool> OnBottomUIToggleRequested;
    
    private void Start()
    {
        OpenCommonInventory();
        CloseCommonInventory();
    }

    public void Init()
    {
        inventoryMap = new Dictionary<InventoryType, Inventory>();

        // 🔥 씬에 있는 Inventory 전부 찾아서 Init
        var inventories = FindObjectsByType<Inventory>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var inv in inventories)
        {
            Register(inv);
            inv.Init();
        }
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
        Debug.Log($"[Register 시도] {inventory.name}");
        inventoryMap[inventory.Type] = inventory;
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

            inv.ResetData();
        }
    }
}
