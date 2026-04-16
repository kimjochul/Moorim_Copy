using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
            return;

        PlayerController owner = GetComponentInParent<PlayerController>();
        owner.ActiveInteractable = interactable;

        PlayerManager pm = PlayerManager.Instance;

        // 현재 선택된 캐릭터가 아니면 예약 실행하지 않음
        if (pm.SelectedUnit != owner) return;

        // 트리거 안에 있다는 사실 항상 기록
        // pm.ActiveInteractable = interactable;

        // 예약된 대상이 아니면 무시
        if (interactable != pm.ReservedInteractable)
            return;

        // 모드에 따라 분기
        if (interactable.Mode == InteractMode.AutoOnEnter)
            pm.ExecuteInteract(interactable); // 트리거 진입 즉시 자동 상호작용
        else 
            pm.ExecuteInteract(interactable); // 첫 클릭으로 이동해서 도착한 경우 → 자동 실행
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
            return;

        PlayerController owner = GetComponentInParent<PlayerController>();
        
        // PlayerManager pm = PlayerManager.Instance;

        // AutoOnEnter 모드: 트리거 퇴장 시 자동으로 상호작용 종료
        if (interactable.Mode == InteractMode.AutoOnEnter)
            interactable.OnInteractEnd(owner);;
            //interactable.OnInteractEnd(pm.SelectedUnit);;
 
        // ActiveInteractable 해제
        if (owner.ActiveInteractable == interactable)
            owner.ActiveInteractable = null;
    }
}