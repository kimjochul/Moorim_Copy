using UnityEngine;
using UnityEngine.AI;

public abstract class GuestEnums
{
    // 손님 입장 / 입장 후 주문 대기, 주문 완료 후 음식 대기, 음식 먹고 수금 / 퇴장
    public enum GuestState { Enter, WaitOrder, WaitFood, Eat, Pay, Exit }

    // 손님 기분 : 매우 나쁨 / 나쁨 / 보통(기본값) / 좋음 / 매우 좋음
    public enum GuestMood { VeryBad, Bad, Neutral, Good, VeryGood }

    // 음식 숙련도에 따른 평가 : 보통(기본값) / 만족 / 대만족 << 임시로 만든 열거형, 음식의 퀄리티 state에 따라 손님 기분이 바뀌게 설정해야함
    public enum FoodQuality { Normal, Fine, Good }
}

// 부모 클래스
public abstract class GuestBase : MonoBehaviour 
{
    [Header("기본 대기 시간 (자식들이 변경 가능)")]
    [SerializeField] protected float orderWaitLimit = 20f;
    [SerializeField] protected float foodMinWaitLimit = 30f;
    [SerializeField] protected float foodMidWaitLimit = 60f;
    [SerializeField] protected float foodMaxWaitLimit = 90f;
    [SerializeField] protected float foodEatLimit = 15f;

    [Header("이동(A*) 설정")]
    [SerializeField] protected NavMeshAgent agent; // 실제로 움직이는 객체
    public Transform targetTable; 
    public Transform exitDoor;
    
    protected float stateTimer = 0f;

    private bool isEnterPathSet = false;
    private bool isExitPathSet = false;
    
    protected GuestEnums.GuestState currentState = GuestEnums.GuestState.Enter; 
    protected GuestEnums.GuestMood currentMood = GuestEnums.GuestMood.Neutral;
    
    void Update()
    {
        GuestFSM();
    }

    private void ChangeState(GuestEnums.GuestState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        Debug.Log($"상태 변경 : {newState}");
    }

    private void GuestFSM()
    {
        switch(currentState)
        {
            case GuestEnums.GuestState.Enter:
                HandleEnterState();
                break;
            case GuestEnums.GuestState.WaitOrder:
                HandleWaitOrderState();
                break;
            case GuestEnums.GuestState.WaitFood:
                HandleWaitFoodState();
                break;
            case GuestEnums.GuestState.Eat:
                HandleEatState();
                break;
            case GuestEnums.GuestState.Pay:
                HandlePayState();
                break;
            case GuestEnums.GuestState.Exit:
                HandleExitState();
                break;
        }
    }

    private void HandleEnterState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isEnterPathSet && targetTable != null)
        {
            agent.SetDestination(targetTable.position);
            isEnterPathSet = true;
            Debug.Log("빈 테이블로 이동 시작");
        }

        // 목적지에 도착했는지 검사
        // pathpending : 경로 계산 중인지 여부, remainingDistance : 목적지까지 남은 거리, stoppingDistance: 목표 지점에서 멈추는 거리
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            ChangeState(GuestEnums.GuestState.WaitOrder);
        }
    }

    protected virtual void HandleWaitOrderState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= orderWaitLimit)
        {
            currentMood = GuestEnums.GuestMood.Bad;
            ChangeState(GuestEnums.GuestState.Exit);
            Debug.Log("주문 대기 시간 초과! 손님의 기분이 매우 나빠져 가게를 나갑니다!");

            // TO DO : 명성도 하락 기능
            // TO DO : 아이콘 변화
        }
    }

    protected virtual void HandleWaitFoodState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodMaxWaitLimit)
        {
            // TO DO : 아이콘 변화
            ChangeState(GuestEnums.GuestState.Exit);
            Debug.Log("음식 대기 1분 30초 경과! 손님이 화를 내며 가게를 나갑니다!");
            
        }
        else if (stateTimer >= foodMidWaitLimit && currentMood == GuestEnums.GuestMood.Bad)
        {
            currentMood = GuestEnums.GuestMood.VeryBad;
            // TO DO : 아이콘 변화
            Debug.Log("음식 대기 60초 경과! 손님의 기분이 매우 나빠졌습니다!");
        }
        else if (stateTimer >= foodMinWaitLimit && currentMood == GuestEnums.GuestMood.Neutral)
        {
            currentMood = GuestEnums.GuestMood.Bad;
            // TO DO : 아이콘 변화
            Debug.Log("음식 대기 30초 경과! 손님의 기분이 나빠졌습니다!");
        }
    }

    protected virtual void HandleEatState()
    {
        // TO DO : 애니메이션 변경
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodEatLimit)
        {
            // TO DO : 애니메이션 변경
            ChangeState(GuestEnums.GuestState.Pay);
        }
    }

    protected virtual void HandlePayState()
    {
        // TO DO : 애니메이션 변경?
        // TO DO : 아이콘 변경?
        // TO DO : 손님 기분 체크
        // TO DO : 손님 기분에 따라 보상 차등
        // TO DO : 돈 지불 -> 자동으로 하루 매출금액에 합산?
        ChangeState(GuestEnums.GuestState.Exit);
    }


    private void HandleExitState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isExitPathSet && exitDoor != null)
        {
            agent.SetDestination(exitDoor.position);
            isExitPathSet = true;
            Debug.Log("손님이 문으로 향해 나갑니다.");
        }

        // 목적지에 도착했는지 검사
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("손님이 가게를 벗어났습니다.");
            // 손님 객체 제거
        }
    }
    
    public void ReceiveFood() // << 음식 받은 걸 어떻게 알게할지?? action 사용?
    {
        if (currentState != GuestEnums.GuestState.WaitFood)
            return;
        
        // TO DO : 음식을 잘못 받을 경우
        // TO DO : 음식을 맞게 받을 경우

        // TO DO : 음식 퀄리티에 따라 손님 기분 변경 >> 이 기분을 HandlePayState()에 전달

    }
}
