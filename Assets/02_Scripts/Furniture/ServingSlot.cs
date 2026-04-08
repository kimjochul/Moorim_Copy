using System.Collections;
using System.Net;
using UnityEngine;
using static Constants;

public class ServingSlot : MonoBehaviour, IInteractable
{
    [Header("��ȣ�ۿ� ĳ����")]
    [SerializeField] 
    private Transform servingPoint;
    [SerializeField]
    private Transform foodPosition; 

    //�丮 ������Ʈ
    private GameObject objFoodPrefab;
    //�ϼ��� �丮 ��ũ��Ʈ �־��� ����
    private GameObject currentFood;
    public float Timer { get; private set; } = 0f;

    [SerializeField] private Inventory inventory;

    public InteractMode Mode => InteractMode.AutoOnEnter;
    
    private void Start()
    {
        inventory.OnItemAdded += OnItemAddedToServing;
    }
    
    private void Update()
    {
        foreach (var slot in inventory.Slots)
        {
            if (slot.Item == null)
                continue;
            
            slot.SpoilTimer += Time.deltaTime;

            if (slot.SpoilTimer >= SPOIL_TIME)
            {
                Debug.Log($"{slot.Item.ItemName} 상함");

                inventory.RemoveItem(slot.Item.ID, 1);

                slot.SpoilTimer = 0f;
            }
        }
    }

    // 시그니처 변경 (내용은 기존 로직 유지하시면서 player 활용)
    public void Interact(PlayerController player)
    {
        // 상호작용 로직
        if (player.PlayerType == PlayerType.Sowol)
        {
            Inventory chefInventory = InventoryManager.Instance.GetInventory(InventoryType.Chef);
            InventoryManager.Instance.OpenInventory(inventory);
            InventoryManager.Instance.OpenInventory(chefInventory);
        }
        else if (player.PlayerType == PlayerType.Myungwol)
        {
            Inventory serverInventory = InventoryManager.Instance.GetInventory(InventoryType.Server);
            InventoryManager.Instance.OpenInventory(inventory);
            InventoryManager.Instance.OpenInventory(serverInventory);
        }
    }

    // 추가
    public void OnInteractEnd(PlayerController player)
    {
        // 종료 시 정리 로직
        if (player.PlayerType == PlayerType.Sowol)
        {
            Inventory chefInventory = InventoryManager.Instance.GetInventory(InventoryType.Chef);
            InventoryManager.Instance.CloseInventory(inventory);
            InventoryManager.Instance.CloseInventory(chefInventory);
        }
        else if (player.PlayerType == PlayerType.Myungwol)
        {
            Inventory serverInventory = InventoryManager.Instance.GetInventory(InventoryType.Server);
            InventoryManager.Instance.CloseInventory(inventory);
            InventoryManager.Instance.CloseInventory(serverInventory);
        }
    }
    
    /// <summary>
    /// 배식대에 음식을 놓는 상황
    /// </summary>
    /// <param name="foodData"></param>
    public void PlaceFood(IItem foodItem, Inventory sourceInven, Slot sourceSlot)
    {
        // inventory.GetItem(foodItem, 1);
        // var slot = inventory.FindSlot(foodItem.ID);
        // slot.SpoilTimer = 0f;
        sourceInven.MoveSlotTo(inventory, sourceSlot, 1);
    }
    
    /// <summary>
    /// 배식대에서 음식을 꺼내는 상황
    /// </summary>
    /// <returns></returns>
    public IItem TakeFood(int slotIndex, Inventory targetInven)
    {
        var slot = inventory.Slots[slotIndex];
        if (slot.Item == null)
        {
            Debug.Log("음식 없음");
            return null;
        }

        IItem item = slot.Item;

        inventory.MoveSlotTo(targetInven, slot, 1);
        inventory.RemoveItem(item.ID, 1);

        if (objFoodPrefab != null)
        {
            Destroy(this.objFoodPrefab);
            objFoodPrefab = null;
        }
        currentFood = null;
        Timer = 0f;
        
        return item;
    }
    
    private void OnItemAddedToServing(Slot slot)
    {
        slot.SpoilTimer = 0f;
        
        // 기존 음식 제거 (중복 방지)
        if (objFoodPrefab != null)
        {
            Destroy(objFoodPrefab);
        }

        // 프리팹 가져오기
        FoodData foodData = slot.Item.Data as FoodData;

        if (foodData == null || foodData.foodPrefab == null)
        {
            Debug.LogWarning("foodPrefab 없음");
            return;
        }

        // 스폰
        objFoodPrefab = Instantiate(
            foodData.foodPrefab,
            foodPosition.position,
            Quaternion.identity,
            this.transform // 선택: 배식대 자식으로
        );
    }
    
    public Vector3 GetInteractPosition()
    {
        return servingPoint.position;
    }
    
    public bool CanInteract(PlayerType playerType)
    {
        return true;
    }
}
