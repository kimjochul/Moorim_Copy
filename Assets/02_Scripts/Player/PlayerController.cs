using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Playables;
using static Constants;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Interact,
    PickUp,
    Give
}

public class PlayerController : MonoBehaviour, IControllable
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public event UnityAction<PlayerState> OnStateChanged;

    public IInteractable ActiveInteractable { get; set; }

    public bool IsMoving
    {
        get
        {
            if (agent == null) return false;
            if (agent.pathPending) return true;
            return agent.hasPath && agent.remainingDistance > agent.stoppingDistance;
        }
    }

    [SerializeField] private PlayerType playerType;
    public PlayerType PlayerType => playerType;

    private NavMeshAgent agent;

    private UnityAction onArrived; // 콜백

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
    public void ChangeState(PlayerState state)
    {
        if (this.CurrentState == state) return;

        this.CurrentState = state;

        this.OnStateChanged?.Invoke(this.CurrentState);
    }

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