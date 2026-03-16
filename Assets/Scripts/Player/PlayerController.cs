using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
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

    private NavMeshAgent agent;

    private Camera mainCamera;
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.mainCamera = Camera.main;
    }
    void Update()
    {
        ClickToMove();
    }
    private void ClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                this.agent.speed = this.moveSpeed;
                this.agent.angularSpeed = this.angularSpeed;
                this.agent.acceleration = this.accelerationSpeed;
                this.agent.SetDestination(hit.point);

                Debug.Log("이동 목표 설정: " + hit.point);
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);
            }
        }
    }
}
