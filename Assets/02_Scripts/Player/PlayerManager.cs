using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Constants;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("플레이어 이동 설정")]
    [Range(1f, 40f)]
    [SerializeField] private float moveSpeed = 10f;
    [Range(60f, 480f)]
    [SerializeField] private float angularSpeed = 120f;
    [Range(4f, 100f)]
    [SerializeField] private float accelerationSpeed = 8f;

    [Header("캐릭터 배열 (0: 명월, 1: 소월)")]
    [SerializeField] private PlayerController[] players;

    // 현재 선택된 유닛
    private PlayerController selectedUnit;
    public PlayerController SelectedUnit => this.selectedUnit;
    public PlayerType CurrentPlayer { get; private set; }

    // 상호작용 예약/활성
    public IInteractable ReservedInteractable { get; set; } // 마우스 클릭으로 예약된 대상
    public IInteractable ActiveInteractable 
    {   get => selectedUnit?.ActiveInteractable;
        set {if (selectedUnit != null) selectedUnit.ActiveInteractable = value;}
    } // 트리거 진입한 대상 -> 중계용으로 변경

    // UI 활성화 중 캐릭터 스왑을 막기 위한 프로퍼티
    public bool BlockSwap { get; set; }

    void Start()
    {
        SelectPlayer(PlayerType.Myungwol);
    }

    public void SelectPlayer(PlayerType playerType)
    {
        int index = (int)playerType;
        if (index >= players.Length || players[index] == null) return;

        // 이전 유닛 해제
        if (selectedUnit != null)
        {
            // ui 닫기 (ActiveInteractable은 유지)
            if (selectedUnit.ActiveInteractable != null)
                selectedUnit.ActiveInteractable.OnInteractEnd(selectedUnit);

            selectedUnit.OnDeselected();
        }

        // 새 캐릭터 선택
        selectedUnit = players[index];
        CurrentPlayer = playerType;
        selectedUnit.OnSelected();

        // 복귀 시 재개
        if (selectedUnit.ActiveInteractable != null && selectedUnit.ActiveInteractable.CanInteract(playerType))
            selectedUnit.ActiveInteractable.Interact(selectedUnit);

    }

    public void MoveSelectedUnitTo(Vector3 destination, UnityAction onArrived = null)
    {
        if (selectedUnit == null)
            return;

        selectedUnit.SetSpeed(moveSpeed, angularSpeed, accelerationSpeed);
        selectedUnit.MoveTo(destination, onArrived);
    }

    public void StopCurrentPlayer()
    {
        selectedUnit?.Stop();
    }

    // 상호작용 실행
    // 트리거 진입 시 or 트리거 안에서 재클릭 시 호출
    // CanInteract 확인 후 Interact를 직접 호출
    public void ExecuteInteract(IInteractable target)
    {
        if (selectedUnit == null) return;
        if (target == null) return;
        if (!target.CanInteract(CurrentPlayer)) return;

        // 이동 중이면 멈추고 상호작용
        selectedUnit.Stop();

        // 상호작용 실행
        target.Interact(selectedUnit);

        // 예약만 초기화 - ActiveInteractable은 트리거 안에 있는 한 유지
        ReservedInteractable = null;
    }

    // 특정 타입의 PlayerController를 반환 (외부에서 참조 필요 시)
    public PlayerController GetPlayer(PlayerType type)
    {
        int index = (int)type;
        if (index < players.Length)
            return players[index];

        return null;
    }
}
