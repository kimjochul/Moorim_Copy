using System;
using UnityEngine;
using UnityEngine.Events;

public class Myungwol : PlayerController
{
    private Inventory inventory;
    
    // 인벤토리 (인벤토리 담당자가 구현 시 타입 교체)
    // TODO: 인벤토리 담당자와 협의 후 실제 Inventory 컴포넌트로 교체
    // [SerializeField] private Inventory inventory;
    // public Inventory Inventory => inventory;

    private void Start()
    {
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Server);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        // 명월 전용 UI 활성화 등
    }
 
    public override void OnDeselected()
    {
        base.OnDeselected();
        // 명월 전용 UI 비활성화 등
    }

    public override void OnSlotClicked(Slot slot)
    {
        base.OnSlotClicked(slot);
        // 명월: 손님에게 음식 전달 등

        if (slot.Owner.Type is InventoryType.MealTable)
            ClickMealTableSlot(slot);
        else if (slot.Owner.Type is InventoryType.Fridge)
            ClickFridgeSlot(slot);
        else if(slot.Owner.Type is InventoryType.Server)
            ClickServerSlot(slot);
        else if(slot.Owner.Type is InventoryType.Guest)
            ClickGuestSlot(slot);
    }

    private void ClickMealTableSlot(Slot slot)
    {
        if (slot.Item == null)
            return;

        var interactable = PlayerManager.Instance.ActiveInteractable;
        if (interactable is ServingSlot servingSlot)
        {
            IItem foodItem = servingSlot.TakeFood(slot.Index, inventory);
            if (foodItem == null)
            {
                Debug.Log("가져올 음식 없음");
                return;
            }
        }
    }
    
    private void ClickFridgeSlot(Slot slot)
    {
        if (slot.Item == null)
            return;
        
        var interactable = PlayerManager.Instance.ActiveInteractable;
        if (interactable is Fridge fridge)
        {
            IItem foodItem = fridge.TakeFood(slot.Index, inventory);
            if (foodItem == null)
            {
                Debug.Log("가져올 음식 없음");
            }
        }
    }
    
    private void ClickGuestSlot(Slot slot)
    {
        if (slot.Item == null)
            return;
        
        var interactable = PlayerManager.Instance.ActiveInteractable;
        if (interactable is GuestInteractable guest)
        {
            IItem foodItem = guest.TakeFood(slot.Index, inventory);
            if (foodItem == null)
            {
                Debug.Log("가져올 음식 없음");
            }
        }
    }

    private void ClickServerSlot(Slot slot)
    {
        if (slot.Item == null)
            return;

        var interactable = PlayerManager.Instance.ActiveInteractable;
        if (interactable is ServingSlot servingSlot)
        {
            if (slot.Owner.Type is InventoryType.Server)
            {
                servingSlot.PlaceFood(inventory, slot);
            }
        }
        else if (interactable is Fridge fridge)
        {
            if (slot.Owner.Type is InventoryType.Server)
            {
                fridge.PlaceFood(inventory, slot);
            }
        }
        else if (interactable is GuestInteractable guest)
        {
            if (slot.Owner.Type is InventoryType.Server)
            {
                guest.PlaceFood(inventory, slot);
            }
        }
    }
}
