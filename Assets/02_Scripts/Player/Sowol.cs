using System;
using Unity.VisualScripting;
using UnityEngine;

public class Sowol : PlayerController
{
    [SerializeField] private CookingStation cookingStation;
    
    private Inventory inventory;
    
    public override void OnSelected()
    {
        base.OnSelected();
    }
    public override void OnDeselected()
    {
        base.OnDeselected();
    }

    // 빈 땅 클릭은 무시, IInteractable 클릭만 이동 허용
    // 조리대 완료 등의 상태 변화는 조리대가 EndInteract()를 콜백으로 호출
    //   → 소월이 CookingStation을 폴링할 필요 없음

    private void Start()
    {
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Chef);
    }
    
    public override void OnSlotClicked(Slot slot)
    {
        if(slot.Owner.Type is InventoryType.IngredientsButMeatOnly or InventoryType.Ingredients)
            ClickIngredientInvenSlot(slot);
        
        if(slot.Owner.Type is InventoryType.Chef)
            ClickChefInvenSlot(slot);
    }

    private void ClickIngredientInvenSlot(Slot slot)
    {
        if (slot.Item == null)
            return;
        
        // 팝업창 나와서 개수 골라서 개수 받는 기능
        
        // 조리 시스템에 개수 넘겨주기
        CookingController.Instance.OnIngredientClicked(slot);
        
        if (slot.Count <= 0)
        {
            slot.SetItem(null, 0);
        }
    }
    
    private void ClickChefInvenSlot(Slot slot)
    {
        if (slot.Item == null)
            return;

        var interactable = PlayerManager.Instance.ActiveInteractable;
        if (interactable is ServingSlot servingSlot)
        {
            if (slot.Owner.Type is InventoryType.Chef)
            {
                servingSlot.PlaceFood(slot.Item);
                inventory.RemoveItem(slot.Item.ID, 1);
            }
        }
    }
}
