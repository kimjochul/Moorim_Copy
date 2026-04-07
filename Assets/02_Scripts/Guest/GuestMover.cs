using UnityEngine;
using UnityEngine.AI;

public class GuestMover : MonoBehaviour
{
    [Header("이동(A*) 설정")]
    private NavMeshAgent agent; // 실제로 움직이는 객체

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // 목적지 설정하고 이동 시작
    public void MoveTo(Vector3 destination)
    {
        agent.enabled = true;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    // 도착했는지 판정
    // threshold: 도착으로 인정할 거리 (기본값은 agent의 stoppingDistance 사용)
    public bool HasArrived(float threshold = -1f)
    {
        if (!agent.enabled) return false;
        if (agent.pathPending) return false;

        float distance = threshold >= 0f ? threshold : agent.stoppingDistance;

        return agent.remainingDistance <= distance;
    }

    // 이동 멈추고 agent 끄기 (착석 시 사용)
    public void StopAndDisable()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    // agent 상태 초기화 (풀링 ResetState에서 호출)
    public void ResetAgent()
    {
        agent.enabled = false;
    }

    // agent 활성화 + 경로 초기화 (Init에서 호출)
    public void EnableAgent()
    {
        agent.enabled = true;
        agent.ResetPath();
        agent.isStopped = false;
        agent.velocity = Vector3.zero;
    }
}
