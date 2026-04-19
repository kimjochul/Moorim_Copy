using System.Collections;
using System.Net;
using UnityEngine;
using static Constants;

public class ServingSlot : MonoBehaviour, IInteractable
{
    [Header("위치 관련 변수")]
    [SerializeField] 
    private Transform servingPoint;
    [SerializeField]
    private Transform foodPosition; 

    // 음식 오브젝트 소환용
    private GameObject objFoodPrefab;
    private GameObject currentFood;

    [SerializeField] private Inventory inventory;
    [SerializeField] private PopupController ui;

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

                if (objFoodPrefab != null)
                {
                    Destroy(objFoodPrefab);
                }
                
                slot.SpoilTimer = 0f;
            }
            //Debug.Log($"{slot.Index}번 슬롯 Spoil Timer : {slot.SpoilTimer}");
        }
    }

    // 시그니처 변경 (내용은 기존 로직 유지하시면서 player 활용)
    public void Interact(PlayerController player)
    {
        Debug.Log("Serving Interact 호출됨");

        // 상호작용 로직
        InventoryManager.Instance.OpenInventory(this.inventory);
        //this.feedback.PlayFeedback();
        UIAnimationManager.Instance.ShowPopFromWorld(this.ui.PanelTransform, this.transform);
        this.inventory.OnCloseRequested += HandleInventoryCloseRequest;
    }

    public void OnInteractEnd(PlayerController player)
    {
        // 종료 시 정리 로직
        HandleInventoryCloseRequest();
    }
    private void HandleInventoryCloseRequest()
    {
        this.inventory.OnCloseRequested -= HandleInventoryCloseRequest;

        UIAnimationManager.Instance.HidePopToWorld(this.ui.PanelTransform, this.transform, 0.3f,
            onComplete: () =>
            {
                InventoryManager.Instance.CloseInventory(this.inventory);
            });
    }

    /// <summary>
    /// 배식대에 음식을 놓는 상황
    /// </summary>
    /// <param name="foodData"></param>
    public void PlaceFood(Inventory sourceInven, Slot sourceSlot)
    {
        int index = sourceInven.MoveSlotTo(inventory, sourceSlot, 1);
        OnItemAddedToServing(inventory.Slots[index]);
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
        
        return item;
    }
    
    private void OnItemAddedToServing(Slot slot)
    {
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
