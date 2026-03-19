using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IControllable
{
    [SerializeField] 
    private GameObject objSelectionArrow;

    private NavMeshAgent agent;

    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
    }

    public virtual void OnSelected()
    {
        if (this.objSelectionArrow != null)
            this.objSelectionArrow.SetActive(true);
        
        Debug.Log(this.gameObject.name + " º±≈√µ ");
    }

    public virtual void OnDeselected()
    {
        if (this.objSelectionArrow != null)
            this.objSelectionArrow.SetActive(false);
        Debug.Log(this.gameObject.name + " «ÿ¡¶µ ");
    }

    public virtual void MoveTo(Vector3 destination)
    {
        this.agent.ResetPath();
        this.agent.velocity = Vector3.zero;
        this.agent.SetDestination(destination);
    }

    public void SetSpeed(float move, float angular, float accel)
    {
        this.agent.speed = move;
        this.agent.angularSpeed = angular;
        this.agent.acceleration = accel;
    }
}
