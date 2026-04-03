using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static Constants;

public class PlayerController : MonoBehaviour, IControllable
{
    // 상태
    private bool isInteracting;
    public bool IsInteracting => isInteracting;

    public bool IsMoving
    {
        get
        {
            if (this.agent == null) return false;
            if (this.agent.pathPending) return true;
            return this.agent.hasPath && this.agent.remainingDistance > this.agent.stoppingDistance;
        }
    }

    [SerializeField] private PlayerType playerType;
    public PlayerType PlayerType => this.playerType;

    private NavMeshAgent agent;

    private UnityAction onArrived; // 콜백

    private IInteractable currentInteractTarget; // 현재 상호작용 대상

    void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        CheckArrival();
    }

    public virtual void OnSlotClicked(Slot slot) { }

    private void CheckArrival()
    {
        // 이동 중이 아니면 체크할 필요 없음
        if (onArrived == null) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                UnityAction callback = onArrived;
                onArrived = null;
                callback?.Invoke();
            }
        }
    }

    public virtual void MoveTo(Vector3 destination, UnityAction onArrived = null)
    {
        // 상호작용 중이면 이동 불가
        if (isInteracting) return;

        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.SetDestination(destination);

        this.onArrived = onArrived;
    }

    public void Stop()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        this.onArrived = null;
    }

    public void SetSpeed(float move, float angular, float accel)
    {
        this.agent.speed = move;
        this.agent.angularSpeed = angular;
        this.agent.acceleration = accel;
    }

    // 상호작용 시작 - PlayerManager 또는 Interactable에서 호출
    public void BeginInteract(IInteractable target)
    {
        isInteracting = true;
        currentInteractTarget = target;
        OnInteractBegin(target);
    }

    // 상호작용 종료 Interactable 완료 시 호출
    public void EndInteract()
    {
        Debug.Log("EndInteract");
        isInteracting = false;
        IInteractable prev = currentInteractTarget;
        currentInteractTarget = null;
        OnInteractEnd(prev);
    }

    // 서브클래스 오버라이드용 훅
    protected virtual void OnInteractBegin(IInteractable target) { }
    protected virtual void OnInteractEnd(IInteractable target) { }

    // 선택/해제
    public virtual void OnSelected()
    {
        Debug.Log($"{gameObject.name} 선택됨");
    }

    public virtual void OnDeselected()
    {
        Debug.Log($"{gameObject.name} 선택 해제");
    }
}