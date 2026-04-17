using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(Collider))]
public class Fridge : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform movePoint;
    [SerializeField] private Inventory inventory;
    
    
    public InteractMode Mode => InteractMode.AutoOnEnter;

    private void Start()
    {
        DayManager dayManager = FindFirstObjectByType<DayManager>();

        dayManager.OnDayEnd += OnDayEnd;
        dayManager.OnDayStart += OnDayStart;
    }

    public void Interact(PlayerController player)
    {
        Debug.Log("Fridge Interact 호출됨");
        
        // 상호작용 로직
        if (player.PlayerType == PlayerType.Sowol)
            InventoryManager.Instance.OpenInventory(inventory);
        else if (player.PlayerType == PlayerType.Myungwol)
            InventoryManager.Instance.OpenInventory(inventory);
    }

    public void OnInteractEnd(PlayerController player)
    {
        // 종료 시 정리 로직
        if (player.PlayerType == PlayerType.Sowol)
            InventoryManager.Instance.CloseInventory(inventory);
        else if (player.PlayerType == PlayerType.Myungwol)
            InventoryManager.Instance.CloseInventory(inventory);
    }
    
    private void OnDayEnd()
    {
        foreach (var slot in inventory.Slots)
        {
            if (slot.Item == null)
                continue;

            slot.FridgeSpoilTimer++;
            Debug.Log($"{slot.Item.ItemName} → {slot.FridgeSpoilTimer}");
        }
    }
    
    private void OnDayStart()
    {
        List<Slot> removeSlots = new List<Slot>();

        foreach (var slot in inventory.Slots)
        {
            if (slot.Item == null)
                continue;

            if (slot.FridgeSpoilTimer >= 3)
            {
                removeSlots.Add(slot);
            }
        }

        foreach (var slot in removeSlots)
        {
            Debug.Log($"{slot.Item.ItemName} 상함 → 제거");

            inventory.RemoveItem(slot.Item.ID, slot.Count);
        }
    }
    
    /// <summary>
    /// 냉장고에 음식을 놓는 상황
    /// </summary>
    /// <param name="foodData"></param>
    public void PlaceFood(Inventory sourceInven, Slot sourceSlot)
    {
        sourceInven.MoveSlotTo(inventory, sourceSlot, 1);
    }
    
    /// <summary>
    /// 냉장고에서 음식을 꺼내는 상황
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
        
        return item;
    }

    public Vector3 GetInteractPosition()
    {
        return movePoint.position;
    }
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return playerType == PlayerType.Sowol;
    }
}