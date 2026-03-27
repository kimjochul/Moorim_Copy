using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum PlayerState { Idle, Move, Interact }

public class PlayerController : MonoBehaviour, IControllable
{
    private PlayerState currentState;
    public PlayerState CurrentState => this.currentState;

    private NavMeshAgent agent;

    private UnityAction onMoveEnd;

    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        CheckArrival();
    }

    private void CheckArrival()
    {
        if (this.CurrentState != PlayerState.Move) return;

        if (!this.agent.pathPending && this.agent.remainingDistance <= this.agent.stoppingDistance)
        {
            if (!this.agent.hasPath || this.agent.velocity.sqrMagnitude == 0f)
            {
                ChangeState(PlayerState.Idle);

                if (this.onMoveEnd != null)
                {
                    this.onMoveEnd.Invoke();
                    this.onMoveEnd = null;
                }
            }
        }
    }

    public virtual void OnSelected() { Debug.Log(this.gameObject.name + " ¼±ÅÃµÊ"); }
    public virtual void OnDeselected() { Debug.Log(this.gameObject.name + " ÇØÁ¦µÊ"); }

    public virtual void ChangeState(PlayerState state)
    {
        if (this.currentState == state) return;

        this.currentState = state;

        switch (this.currentState)
        {
            case PlayerState.Idle: 
                break;
            case PlayerState.Move: 
                break;
            case PlayerState.Interact:
                break;
        }
    }

    public virtual void MoveTo(Vector3 destination, UnityAction action)
    {
        ChangeState(PlayerState.Move);

        this.agent.ResetPath();
        this.agent.velocity = Vector3.zero;
        this.agent.SetDestination(destination);

        this.onMoveEnd = action;
    }

    public virtual void SetSpeed(float move, float angular, float accel)
    {
        this.agent.speed = move;
        this.agent.angularSpeed = angular;
        this.agent.acceleration = accel;
    }
}