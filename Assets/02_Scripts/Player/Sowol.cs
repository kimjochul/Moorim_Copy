using System;
using Unity.VisualScripting;
using UnityEngine;

public class Sowol : PlayerController
{
    // 인벤토리 (인벤토리 담당자가 구현 시 타입 교체)
    // TODO: 인벤토리 담당자와 협의 후 실제 Inventory 컴포넌트로 교체
    // [SerializeField] private Inventory inventory;
    // public Inventory Inventory => inventory;
 
    public override void OnSlotClicked(Slot slot)
    {
        base.OnSlotClicked(slot);
        // 소월: 재료 이동 등
    }
    
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
}
