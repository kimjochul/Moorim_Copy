using UnityEngine;

public class ThiefGuyGuest : GuestBase
{
    private bool isCaught = false; // 잡혔는지 체크용

    void Awake()
    {
        orderWaitLimit = 40f;
        foodMaxWaitLimit = 120f;
    }

    public override void ResetState()
    {
        base.ResetState();
        isCaught = false; // 풀에서 다시 나올 때 잡힘 상태도 초기화
    }
    
    protected override void HandleWaitOrderState()
    {
        base.HandleWaitOrderState();
    }

    protected override void HandleWaitFoodState()
    {
        base.HandleWaitFoodState();
    }
    
    protected override void HandleEatState()
    {
        // TO DO : 애니메이션 변경
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodEatLimit)
        {            
            ChangeState(Constants.GuestState.Exit);
        }
    }

    protected override void HandlePayState()
    {
        base.HandlePayState();
    }

    protected override int CalculateFinalPrice()
    {
        return base.CalculateFinalPrice();
    }

    // 플레이어가 도망가는 도둑과 상호작용(F키/클릭)했을 때 호출할 메소드
    public void CatchThief()
    {
        if (currentState == Constants.GuestState.Exit && !isCaught)
        {
            isCaught = true;

            // 도둑을 잡으면 추가 보너스 지급?
            ChangeState(Constants.GuestState.Pay);
        }
    }
}
