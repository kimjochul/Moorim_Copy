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

    protected override void OnInteractBegin(IInteractable target)
    {
        // 조리대 → 조리대가 소월 인벤토리를 확인하고 완성 음식을 넣어줌
        // 냉장고 → 냉장고 인벤토리 + 자신 인벤토리 UI
        // 배식구 → 배식구 인벤토리 + 자신 인벤토리 UI
        Debug.Log($"소월 상호작용 시작: {target}");
    }
    
    protected override void OnInteractEnd(IInteractable target)
    {        
        Debug.Log($"소월 상호작용 종료: {target}");
    }
 
    // 소월의 이동 제한은 PlayerInput에서 처리
    // 빈 땅 클릭은 무시, IInteractable 클릭만 이동 허용
    // 조리대 완료 등의 상태 변화는 조리대가 EndInteract()를 콜백으로 호출
    //   → 소월이 CookingStation을 폴링할 필요 없음
    
    public override void OnSlotClicked(Slot slot)
    {
        if(slot.Owner.Type is InventoryType.IngredientsButMeatOnly or InventoryType.IngredientsButMeatOnly)
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
        
    }
}
