using UnityEngine;
using UnityEngine.Events;

public class Myungwol : PlayerController
{
    // 인벤토리 (인벤토리 담당자가 구현 시 타입 교체)
    // TODO: 인벤토리 담당자와 협의 후 실제 Inventory 컴포넌트로 교체
    // [SerializeField] private Inventory inventory;
    // public Inventory Inventory => inventory;
 
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
 
    protected override void OnInteractBegin(IInteractable target)
    {
        // 배식구 상호작용 → 배식구 인벤토리 + 자신 인벤토리 UI
        // 손님 상호작용 → 인벤토리 열어서 슬롯 클릭 시 음식 데이터 전달
        Debug.Log($"명월 상호작용 시작: {target}");
    }
 
    protected override void OnInteractEnd(IInteractable target)
    {
        Debug.Log($"명월 상호작용 종료: {target}");
    }

    public override void OnSlotClicked(Slot slot)
    {
        base.OnSlotClicked(slot);
        // 명월: 손님에게 음식 전달 등
    }
}
