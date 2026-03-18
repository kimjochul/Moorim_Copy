using UnityEngine;

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
    
    protected float stateTimer = 0f;
    
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
            case GuestEnums.GuestState.Pay:// 1회성 행동이라 update 밖으로 빼낼지?
                HandlePayState();
                break;
            case GuestEnums.GuestState.Exit:// 1회성 행동이라 update 밖으로 빼낼지?
                HandleExitState();
                break;
        }
    }

    private void HandleEnterState()
    {
        // TO DO :  A* 사용해서 빈 테이블로 이동
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
        // TO DO : 손님 기분에 따라 보상 차등?
        // TO DO : 돈 지불 -> 자동으로 하루 매출금액에 합산?
        ChangeState(GuestEnums.GuestState.Exit);
    }


    private void HandleExitState()
    {
        // TO DO :  A* 사용해서 문 밖으로 이동
    }
    
    public void ReceiveFood() // << 음식 받은 걸 어떻게 알게할지?? action 사용?
    {
        if (currentState != GuestEnums.GuestState.WaitFood)
            return;
        
        // TO DO : 음식을 잘못 받을 경우
        // TO DO : 음식을 맞게 받을 경우

        // TO DO : 음식 퀄리티에 따라 손님 기분 변경 >> 이 기분을 외부에 전달

    }
}
