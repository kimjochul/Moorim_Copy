using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Constants;

public class PlayerInput : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 글로벌 UI 입력 (어떤 상태에서든 항상 처리)
        GlobalUIInput();
 
        // 캐릭터 전환
        PlayerSelection();
 
        // 마우스 클릭
        if (Input.GetMouseButtonDown(0))
            MouseClick();
    }

    private void GlobalUIInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // UIManager.Instance.인터페이스/도감 여는 메소드
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // UIManager.Instance.인터페이스/도감 닫는 메소드
        }
    }

    // 캐릭터 전환
    private void PlayerSelection()
    {
        if (PlayerManager.Instance.BlockSwap) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PlayerManager.Instance.SelectPlayer(PlayerType.Myungwol);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            PlayerManager.Instance.SelectPlayer(PlayerType.Sowol);
    }

    // 마우스 클릭
    private void MouseClick()
    {
        // UI 위를 클릭했으면 게임, 월드 클릭 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            return;

        IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
 
        if (interactable != null)
            HandleInteractableClick(interactable);
        else
            HandleGroundClick(hit.point);
    }

    // IInteractable 오브젝트 클릭
    // 이미 트리거 안에 있으면 → 이동 없이 바로 재실행 (ClickToInteract 모드)
    // 트리거 밖이면 → 예약 후 이동
    private void HandleInteractableClick(IInteractable interactable)
    {
        PlayerType currentPlayer = PlayerManager.Instance.CurrentPlayer;

        if (!interactable.CanInteract(currentPlayer))
            return;
        
        if (interactable.Mode == InteractMode.Immediate)
        {
            interactable.Interact(PlayerManager.Instance.SelectedUnit);
            return;
        }
        
        // 이미 해당 대상의 트리거 안에 있으면 이동 없이 바로 상호작용
        if (PlayerManager.Instance.ActiveInteractable == interactable)
        {
            PlayerManager.Instance.ExecuteInteract(interactable);
            return;
        }

        // 트리거 밖 -> 예약 등록 후 이동
        PlayerManager.Instance.ReservedInteractable = interactable;
        PlayerManager.Instance.MoveSelectedUnitTo(interactable.GetInteractPosition());
    }

    // 명월 전용 / 빈 땅 클릭 시 이동
    private void HandleGroundClick(Vector3 point)
    {
        if (PlayerManager.Instance.CurrentPlayer == PlayerType.Sowol) return;

        PlayerManager.Instance.ReservedInteractable = null;
        PlayerManager.Instance.MoveSelectedUnitTo(point);
    }
}