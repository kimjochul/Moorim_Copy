using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Constants;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("캐릭터 이동 설정")]
    [Range(1f, 40f)]
    [SerializeField] 
    private float moveSpeed = 10f;
    [Range(60f, 480f)]
    [SerializeField] 
    private float angularSpeed = 120f;
    [Range(4f, 100f)]
    [SerializeField] 
    private float accelerationSpeed = 8f;

    [SerializeField] private PlayerController[] players;

    private IControllable selectedUnit;
    public IControllable SelectedUnit => this.selectedUnit;
    public PlayerType CurrentPlayer { get; private set; }

    public IInteractable ReservedInteractable { get; set; } // 마우스로 클릭한 물체
    public IInteractable ActiveInteractable { get; set; } // 충돌한 물체

    void Start()
    {
        SelectPlayer(PlayerType.Myungwol);
    }

    public void SelectPlayer(PlayerType playerType)
    {
        int index = (int)playerType;
        if (index >= this.players.Length || this.players[index] == null) return;

        if (this.selectedUnit != null)
            this.selectedUnit.OnDeselected();

        this.selectedUnit = this.players[index];
        this.CurrentPlayer = playerType;
        this.selectedUnit.OnSelected();

        Debug.Log($"{playerType} 캐릭터 선택");
    }

    public void MoveSelectedUnitTo(Vector3 destination, UnityAction action = null)
    {
        if (this.selectedUnit != null)
        {
            this.selectedUnit.SetSpeed(this.moveSpeed, this.angularSpeed, this.accelerationSpeed);
            this.selectedUnit.MoveTo(destination, action);
        }
    }
    public void StopCurrentPlayer()
    {
        this.selectedUnit?.ChangeState(PlayerState.Idle);
    }
}
