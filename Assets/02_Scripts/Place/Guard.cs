using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour
{
    [Header("가드 설정")]
    [SerializeField] private float suppressDistance = 1f; // 제압 가능한 도달 거리

    private NavMeshAgent agent;
    private Vector3 idlePosition;
    private ViolenceGuest currentTarget;

    private enum GuardState { Idle, Chasing, Returning }
    private GuardState currentState = GuardState.Idle;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        idlePosition = transform.position; // 배치된 시작 위치를 기본 대기 장소로 설정
    }

    void OnEnable()
    {
        // 난동 시작 이벤트 구독
        ViolenceGuest.OnViolenceStarted += HandleViolenceStarted;
    }

    void OnDisable()
    {
        // 난동 시작 이벤트 구독 해제
        ViolenceGuest.OnViolenceStarted -= HandleViolenceStarted;
    }

    private void HandleViolenceStarted(ViolenceGuest guest)
    {
        // Guard가 대기 중이거나 제자리로 돌아가는 중일 때 새로운 난동꾼이 생기면 타겟 변경
        if (currentState == GuardState.Idle || currentState == GuardState.Returning)
        {
            currentTarget = guest;
            currentState = GuardState.Chasing;
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    void Update()
    {
        GuardFSM();
    }

    private void GuardFSM()
    {
        switch (currentState)
        {
            case GuardState.Idle:
                // 대기 중 처리 (필요시 대기 애니메이션 재생 등)
                break;

            case GuardState.Chasing:
                // 타겟이 맵에서 사라졌거나, 이미 플레이어에 의해 제압된 경우
                if (currentTarget == null || !currentTarget.isCausingTrouble)
                {
                    ReturnToPost();
                    return;
                }

                // 타겟의 현재 위치로 계속 갱신 (손님이 약간 움직일 가능성 대비)
                agent.SetDestination(currentTarget.transform.position);

                // 목표물에 충분히 가까워졌는지 확인 (닿았는지 판정)
                if (Vector3.Distance(transform.position, currentTarget.transform.position) <= suppressDistance)
                {
                    currentTarget.SuppressViolence(); // 타겟 제압[cite: 8]
                    ReturnToPost(); // 제압 후 대기소로 복귀
                }
                break;

            case GuardState.Returning:
                // 대기 위치로 도착했는지 확인
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = GuardState.Idle;
                }
                break;
        }
    }

    private void ReturnToPost()
    {
        currentTarget = null;
        currentState = GuardState.Returning;
        agent.SetDestination(idlePosition);
    }
}