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
    public IInteractable ActiveInteractable { get; set; } // 트리거 진입한 대상

    void Start()
    {
        SelectPlayer(PlayerType.Myungwol);
    }

    public void SelectPlayer(PlayerType playerType)
    {
        int index = (int)playerType;
        if (index >= this.players.Length || this.players[index] == null) return;

        // 이전 유닛 해제
        if (this.selectedUnit != null)
            this.selectedUnit.OnDeselected();

        this.selectedUnit = this.players[index];
        this.CurrentPlayer = playerType;
        this.selectedUnit.OnSelected();
    }

    public void MoveSelectedUnitTo(Vector3 destination, UnityAction onArrived = null)
    {
        if (this.selectedUnit == null)
            return;

            this.selectedUnit.SetSpeed(this.moveSpeed, this.angularSpeed, this.accelerationSpeed);
            this.selectedUnit.MoveTo(destination, onArrived);
    }

    public void StopCurrentPlayer()
    {
        this.selectedUnit?.Stop();
    }

    // 상호작용 실행
    // PlayerInteractor 트리거 진입 시 호출 (첫 도착)
    // PlayerInput에서 호출 (트리거 안에서 재클릭)
    public void ExecuteInteract(IInteractable target)
    {
        if (this.selectedUnit == null) return;
        if (this.selectedUnit.IsInteracting) return;
        if (target == null) return;

        if (!target.CanInteract(this.CurrentPlayer)) return;

        // 이동 중이면 멈추고 상호작용
        this.selectedUnit.Stop();

        // 상호작용 실행
        this.selectedUnit.BeginInteract(target);
        target.Interact(this.selectedUnit);

        // 예약만 초기화 - ActiveInteractable은 트리거 안에 있는 한 유지
        this.ReservedInteractable = null;
    }

    // 특정 타입의 PlayerController를 반환 (외부에서 참조 필요 시)
    public PlayerController GetPlayer(PlayerType type)
    {
        int index = (int)type;
        if (index < this.players.Length)
            return this.players[index];

        return null;
    }
}
