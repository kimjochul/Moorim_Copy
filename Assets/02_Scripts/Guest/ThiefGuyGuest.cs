using UnityEngine;

public class ThiefGuyGuest : GuestBase
{
    private bool isCaught = false; // 잡혔는지 체크용
    private bool isEscape = false; // 도망치는 중인지 체크중

    void Awake()
    {
        orderMaxWaitLimit = 40f;
        foodMaxWaitLimit = 120f;
    }

    public override void ResetState()
    {
        base.ResetState();
        recipeUnlockManager = RecipeUnlockManager.Instance;
        isCaught = false; // 풀에서 다시 나올 때 잡힘 상태도 초기화
        isEscape = false;
        
        if (guestAnim != null)
            guestAnim.OnCatchedFinished -= OnResumeEscape;
        
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
        base.HandleEatState();
    }

    protected override void HandlePayState()
    {
        // 1. 손 내리기 애니메이션이 아직 재생 중이면 대기
        if (isPlayingHandDown) return;

        // 2. 손을 다 내렸다면 결제 UI 띄우지 말고 냅다 일어납니다 (먹튀!)
        if (!isEscape)
        {
            isEscape = true;
            isPlayingStandUp = true;
            guestAnim.PlayStandUp();
            // StandUp이 완료되면 부모(GuestBase)에 등록된 콜백이 알아서 Exit 상태로 바꿔줌.
        }
    }

    // 플레이어가 도망가는 도둑과 상호작용(F키/클릭)했을 때 호출할 메소드
    public void CatchThief()
    {
        Debug.Log("CatchThief 클릭됨");
        
        // 도망가는 중(Exit)일 때만 잡을 수 있음
        if (currentState == Constants.GuestState.Exit && !isCaught)
        {
            Debug.Log("<color=cyan>도둑 검거 성공!! 음식값을 받아냅니다.</color>");
            isCaught = true;

            // Pay 상태로 돌려보내지 말고 여기서 즉시 돈과 명성을 올려줌
            if (myOrder != null && myOrder.orderedFood != null)
            {
                EconomyManager.Instance.AddMoney(myOrder.orderedFood.price); // 음식값 징수
                EconomyManager.Instance.AddFame(5); // 검거 보너스 명성치 
            }
            
            // 1. 이동 즉시 정지 (NavMeshAgent 끄기)
            mover.StopAndDisable();

            // 2. 애니메이션이 끝난 후 다시 도망가도록 이벤트 구독
            guestAnim.OnCatchedFinished += OnResumeEscape;

            // 3. 잡히는 애니메이션 재생 (Guest_Catched 상태로 진입)
            guestAnim.PlayCatched();
        }
    }
    
    private void OnResumeEscape()
    {
        // 이벤트 중복 실행 방지를 위해 구독 해제
        guestAnim.OnCatchedFinished -= OnResumeEscape;

        // 다시 걷기 애니메이션 재생
        guestAnim.PlayWalk();

        // NavMeshAgent 켜고 다시 출구로 이동
        if (exitDoor != null)
        {
            mover.EnableAgent();
            mover.MoveTo(exitDoor.position);
        }
    }
    
    
}
