using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IControllable
{
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
        if (this.onMoveEnd == null) return;

        if (!this.agent.pathPending && this.agent.remainingDistance <= this.agent.stoppingDistance)
        {
            if (!this.agent.hasPath || this.agent.velocity.sqrMagnitude == 0f)
            {
                this.onMoveEnd?.Invoke();

                this.onMoveEnd = null;
            }
        }
    }
    public virtual void OnSelected()
    {
        Debug.Log(this.gameObject.name + " 선택됨");
    }

    public virtual void OnDeselected()
    {
        Debug.Log(this.gameObject.name + " 해제됨");
    }

    public virtual void MoveTo(Vector3 destination, UnityAction action)
    {
        this.agent.ResetPath();
        this.agent.velocity = Vector3.zero;
        this.agent.SetDestination(destination);

        //이동 끝난 후 이벤트 등록
        this.onMoveEnd = action;
    }

    public void SetSpeed(float move, float angular, float accel)
    {
        this.agent.speed = move;
        this.agent.angularSpeed = angular;
        this.agent.acceleration = accel;
    }
}
