using System;
using Unity.VisualScripting;
using UnityEngine;

public class Sowol : PlayerController
{
    [SerializeField] private CookingStation cookingStation;
    
    public override void OnSelected()
    {
        base.OnSelected();
    }
    public override void OnDeselected()
    {
        base.OnDeselected();
    }
    
    private void LateUpdate()
    {
        CheckInteractState();
    }

    private void CheckInteractState()
    {
        if(CurrentState == PlayerState.Interact && cookingStation.CurrentState != CookingState.Cooking)
            ChangeState(PlayerState.Idle);
    }

    public override void OnSlotClicked(Slot slot)
    {
        if(slot.Owner.Type == InventoryType.IngredientsButMeatOnly || slot.Owner.Type == InventoryType.IngredientsButMeatOnly)
            ClickIngredientInvenSlot(slot);
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
}
