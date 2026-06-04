using UnityEngine;
using UnityEngine.AI;

public class ThiefGuyGuest : GuestBase
{
    private bool isCaught = false; 
    private bool isEscape = false; 

    [Header("술래잡기 설정")]
    public float fleeDetectRadius = 100f; 
    public float fleeDistance = 100f;     
    private float fleeTimer = 0f;       

    void Awake()
    {
        orderMaxWaitLimit = 40f;
        foodMaxWaitLimit = 120f;
    }

    public override void ResetState()
    {
        base.ResetState();
        recipeUnlockManager = RecipeUnlockManager.Instance;
        isCaught = false; 
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
        if (isPlayingHandDown) return;

        if (!isEscape)
        {
            isEscape = true;
            isPlayingStandUp = true;
            guestAnim.PlayStandUp();
        }
    }

    protected override void HandleExitState()
    {
        if (isCaught) return; 

        base.HandleExitState(); 

        fleeTimer -= Time.deltaTime;
        if (fleeTimer > 0f) return;

        PlayerController chaser = PlayerManager.Instance.SelectedUnit; 
        if (chaser == null) return;

        float distToPlayer = Vector3.Distance(transform.position, chaser.transform.position);

        if (distToPlayer < fleeDetectRadius)
        {
            Vector3 bestPos = GetBestEscapePoint(chaser.transform.position);
            
            // 급브레이크를 거는 EnableAgent() 삭제! MoveTo만 호출해서 부드럽게 방향만 전환
            mover.MoveTo(bestPos);
            fleeTimer = 0.05f; 
        }
        else
        {
            if (Gate.isOpen)
            {
                if (exitDoor != null)
                {
                    mover.MoveTo(exitDoor.position);
                }
                fleeTimer = 1.0f; 
            }
            else
            {
                mover.StopAndDisable();
                fleeTimer = 0.5f; 
            }
        }
    }

    private Vector3 GetBestEscapePoint(Vector3 chaserPos)
    {
        Vector3 bestPoint = transform.position;
        float maxScore = -9999f;

        Vector3 awayDir = (transform.position - chaserPos).normalized;
        awayDir.y = 0; 

        float[] angles = { 0f, 45f, -45f, 90f, -90f, 135f, -135f };

        foreach (float angle in angles)
        {
            Vector3 checkDir = Quaternion.Euler(0, angle, 0) * awayDir;
            Vector3 targetPos = transform.position + checkDir * fleeDistance;

            NavMeshHit hit;
            bool isBlocked = NavMesh.Raycast(transform.position, targetPos, out hit, NavMesh.AllAreas);
            
            Vector3 actualPos = isBlocked ? hit.position : targetPos;
            float score = Vector3.Distance(actualPos, chaserPos);

            if (isBlocked) 
            {
                score -= 10.0f; 
            }

            if (score > maxScore)
            {
                maxScore = score;
                bestPoint = actualPos;
            }
        }

        return bestPoint;
    }

    public void CatchThief()
    {
        if (currentState == Constants.GuestState.Exit && !isCaught)
        {
            Debug.Log("<color=cyan>도둑 검거 성공!! 음식값을 받아냅니다.</color>");
            isCaught = true;

            if (myOrder != null && myOrder.orderedFood != null)
            {
                EconomyManager.Instance.AddMoney(myOrder.orderedFood.price); 
                EconomyManager.Instance.AddFame(5); 
            }
            
            mover.StopAndDisable();

            guestAnim.OnCatchedFinished += OnResumeEscape;
            guestAnim.PlayCatched();
        }
    }
    
    private void OnResumeEscape()
    {
        guestAnim.OnCatchedFinished -= OnResumeEscape;

        guestAnim.PlayWalk();

        if (exitDoor != null)
        {
            mover.EnableAgent(); 
            mover.MoveTo(exitDoor.position);
        }
    }
}