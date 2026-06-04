using System;
using System.Collections.Generic;
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

    // 캐릭터가 스왑될 때마다 UI에 알려줄 이벤트
    public event Action<PlayerType> OnPlayerSwapped;

    private Dictionary<PlayerType, IInteractable> lastInteractables = new();
    
    // 달리기 중인지 
    public bool IsRunMode { get; private set; } = false;
    
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        SelectPlayer(PlayerType.Myungwol);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        players[0] = FindFirstObjectByType<Myungwol>();
        players[1] = FindFirstObjectByType<Sowol>();
        SelectPlayer(PlayerType.Myungwol);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
            {
                lastInteractables[CurrentPlayer] = selectedUnit.ActiveInteractable;
                selectedUnit.ActiveInteractable.OnInteractEnd(selectedUnit);
            }

            selectedUnit.OnDeselected();
        }

        bool resumed = false;
        
        // 새 캐릭터 선택
        selectedUnit = players[index];
        CurrentPlayer = playerType;
        selectedUnit.OnSelected();

        if (lastInteractables.TryGetValue(playerType, out var last))
        {
            if (last != null && last.CanInteract(playerType))
            {
                if (last is MonoBehaviour mb && mb.gameObject.activeInHierarchy)
                {
                    selectedUnit.ActiveInteractable = last;
                    last.Interact(selectedUnit);
                    resumed = true;
                }
            }
        }
        
        // 복귀 시 재개
        if (!resumed && selectedUnit.ActiveInteractable != null && selectedUnit.ActiveInteractable.CanInteract(playerType))
            selectedUnit.ActiveInteractable.Interact(selectedUnit);
        
        OnPlayerSwapped?.Invoke(playerType);
    }

    public void MoveSelectedUnitTo(Vector3 destination, UnityAction onArrived = null)
    {
        if (selectedUnit == null)
            return;
        
        float currentMoveSpeed = IsRunMode ? moveSpeed * 1.5f : moveSpeed;
        
        selectedUnit.SetSpeed(currentMoveSpeed, angularSpeed, accelerationSpeed);
        selectedUnit.MoveTo(destination, onArrived);
    }
    
    public void ToggleRunMode()
    {
        IsRunMode = !IsRunMode; 
        
        if (selectedUnit != null)
        {
            float currentMoveSpeed = IsRunMode ? moveSpeed * 1.5f : moveSpeed;
            selectedUnit.SetSpeed(currentMoveSpeed, angularSpeed, accelerationSpeed);
            
            // 이동 중에 O키를 눌렀다면 애니메이션 즉시 갱신
            if (selectedUnit.IsMoving)
            {
                if (IsRunMode)
                    selectedUnit.ChangeState(PlayerState.Run);
                else
                    selectedUnit.ChangeState(PlayerState.Walk);
            }
        }
    }
    
    public void TeleportSelectedUnit(Vector3 destination)
    {
        if (selectedUnit == null) return;

        var agent = selectedUnit.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.Warp(destination);
            selectedUnit.ChangeState(PlayerState.Idle);
        }
        else
        {
            selectedUnit.transform.position = destination;
        }
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

        // 상호작용 애니메이션 실행
        selectedUnit.ChangeState(PlayerState.Interact);

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
