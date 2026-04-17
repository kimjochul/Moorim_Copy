using System;
using UnityEngine;
using static Constants;

// 부모 클래스
public abstract class GuestBase : MonoBehaviour
{
    [Header("기본 대기 시간 (자식들이 변경 가능)")]
    [SerializeField] protected float orderMinWaitLimit = 15f;
    [SerializeField] protected float orderMaxWaitLimit = 20f;
    [SerializeField] protected float foodMinWaitLimit = 30f;
    [SerializeField] protected float foodMidWaitLimit = 60f;
    [SerializeField] protected float foodMaxWaitLimit = 90f;
    [SerializeField] protected float foodEatLimit = 15f;
    [SerializeField] protected float menuChooseTime = 3f;
    [SerializeField] protected float payWaitTime = 3f;

    [Header("데이터 연결")]
    [SerializeField] private RecipeList menuBoard;
    [SerializeField] private GuestRewardData rewardData;

    private GuestMover mover;
    private GuestManager guestManager;
    private GuestBubbleUI bubbleUI;
    private GuestAnimation guestAnim;

    public Transform exitDoor;
    public Chair myChair;

    // 손님 런타임 정보
    public int individualId;
    public GuestAppearance myApperance;
    public GuestOrderData myOrder = new GuestOrderData();

    protected float stateTimer = 0f;

    // 상태 플래그
    private bool isEnterPathSet = false;
    private bool isExitPathSet = false;
    private bool isOrderTaken = false;

    // 음식 모델
    private GameObject currentFoodModel;
    
    // 상태 & 기분
    public GuestState currentState { get; private set; }
    public GuestMood CurrentMood { get; protected set; } = GuestMood.Neutral;
    private FoodQuality foodQuality = FoodQuality.Normal;

    private bool isAngryExiting = false;  // VeryBad 퇴장 중 플래그
    private bool isPlayingHandDown = false;
    private bool isPlayingStandUp = false;

    // Action 이벤트
    public event Action<GuestBase, GuestState> OnGuestStateChanged; // 상태 변경 알림용
    public event Action<GuestBase, GuestMood> OnGuestMoodChanged; // 기분 변경 알림용
    public event Action<GuestBase> OnGuestReadyToOrder; // 주문 대기 중일 때
    public event Action<GuestBase, GuestOrderData> OnOrderPlaced; // UI 주문 리스트용
    public event Action<GuestBase> OnOrderServed; // 서빙 완료 이벤트
    public event Action<GuestBase> OnGuestReadyToPay; // 계산 중일 때
    public event Action OnGuestPaid; // 계산 완료 시 (Pay 상태에서만)
    public event Action OnGuestExited;  // 퇴장 시 (모든 Exit에서)

    void Awake()
    {
        mover = GetComponent<GuestMover>();
        bubbleUI = GetComponentInChildren<GuestBubbleUI>();
    }
    
    // 풀에서 꺼낼 때 호출. 재사용 시 이전 생애의 모든 상태를 깨끗이 초기화해 버그 방지
    public virtual void ResetState()
    {
        if (mover == null)
            mover = GetComponent<GuestMover>();
        if (bubbleUI == null)
            bubbleUI = GetComponentInChildren<GuestBubbleUI>();
        if (guestManager != null)
            guestManager.LeaveWaiting(this);
        
        // FSM 플래그 — 이게 안 되면 Enter/Exit에서 경로를 안 잡음
        isEnterPathSet = false;
        isExitPathSet = false;
        isOrderTaken = false;

        // 타이머 & 상태값
        stateTimer = 0f;
        CurrentMood = GuestMood.Neutral;
        foodQuality = FoodQuality.Normal;

        // 말풍선
        if (bubbleUI != null)
            bubbleUI.HideAll();

        // 주문 데이터
        if (myOrder == null)
            myOrder = new GuestOrderData();
        else
            myOrder.Clear();

        // 의자 반환 (이전 손님이 앉았던 의자가 점유 상태로 남는 것 방지)
        if (myChair != null)
        {
            myChair.Empty();
            myChair = null;
        }

        // 음식 모델 제거
        if (currentFoodModel != null)
        {
            Destroy(currentFoodModel);
            currentFoodModel = null;
        }

        if (mover != null)
            mover.ResetAgent();

        if (guestAnim != null)
            guestAnim.ResetAnimation();

        isAngryExiting = false;
        isPlayingHandDown = false;
        isPlayingStandUp = false;

        // 이벤트 구독 전부 해제
        // 안 하면 이전 생애의 리스너가 남아서 이벤트가 중복 호출됨
        OnGuestStateChanged = null;
        OnGuestMoodChanged = null;
        OnGuestReadyToOrder = null;
        OnOrderPlaced = null;
        OnGuestReadyToPay = null;
        // OnGuestServedAndPaid = null;
        OnGuestPaid = null;
        OnGuestExited = null;
        OnOrderServed = null;
    }
    
    // 객체가 생성될 때 매니저에서 딱 한 번 호출해줄 초기화 메소드
    public virtual void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        individualId = id;
        myApperance = appearance;
        exitDoor = exit;
        guestManager = manager;
        
        if (myOrder == null)
            myOrder = new GuestOrderData();
        else
            myOrder.Clear();

        gameObject.SetActive(true);

        mover.EnableAgent();

        // 말풍선 UI가 이벤트 구독
        if (bubbleUI != null)
        {
            bubbleUI.HideAll();
           bubbleUI.Bind(this);
        }

        if (guestAnim == null)
            guestAnim = GetComponentInChildren<GuestAnimation>();

        if (guestAnim != null)
        {
            guestAnim.ResetAnimation();
            guestAnim.Bind(this);
            BindAnimCallbacks();
        }
        
        ChangeState(GuestState.Enter);
    }

    void Update()
    {
        GuestFSM();
    }

    protected void ChangeState(Constants.GuestState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        OnGuestStateChanged?.Invoke(this, currentState);
    }

    private void GuestFSM()
    {
        switch(currentState)
        {
            case GuestState.Enter:      HandleEnterState();     break;
            case GuestState.Waiting:    HandleWaitingState();   break;
            case GuestState.WaitOrder:  HandleWaitOrderState(); break;
            case GuestState.WaitFood:   HandleWaitFoodState();  break;
            case GuestState.Eat:        HandleEatState();       break;
            case GuestState.Pay:        HandlePayState();       break;
            case GuestState.Exit:       HandleExitState();      break;
        }
    }

    private void BindAnimCallbacks()
    {
        // 앉기 완료 → WaitOrder 진입
        guestAnim.OnSitDownFinished += () =>
        {
            if (currentState == GuestState.Enter || currentState == GuestState.WaitOrder)
            {
                // 이미 WaitOrder면 버블 UI만 갱신됨
                // Enter에서 도착 후 앉기가 끝났으므로 진짜 WaitOrder 시작
            }
        };

        // HandUp 완료 → 실제 Eating 시작 (이미 Eat 상태)
        guestAnim.OnHandUpFinished += () =>
        {
            // Eat 상태의 타이머가 이 시점부터 의미를 가짐
            // (HandleEatState에서 타이머 체크)
        };

        // HandDown 완료 → StandUp 시작
        guestAnim.OnHandDownFinished += () =>
        {
            isPlayingHandDown = false;
            if (!isAngryExiting)
            {
                isPlayingStandUp = true;
                guestAnim.PlayStandUp();
            }
        };

        // StandUp 완료 → Exit 상태 전환
        guestAnim.OnStandUpFinished += () =>
        {
            isPlayingStandUp = false;
            ChangeState(GuestState.Exit);
        };

        // Angry 완료 → 일어서서 퇴장
        guestAnim.OnAngryFinished += () =>
        {
            if (isAngryExiting)
            {
                guestAnim.PlayStandUp();
            }
        };
    }
    
    private void HandleEnterState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isEnterPathSet)
        {
            // 매니저에게 랜덤 빈자리 요청
            myChair = SeatManager.Instance.FindEmptyChair();

            if (myChair != null)
            {
                myChair.Reserve(this); // 자리 찜
                mover.MoveTo(myChair.sitPoint.position);
                isEnterPathSet = true;
            }
            else
            {
                // 빈자리가 없다면 웨이팅 시도
                Vector3 waitPos;
                if (guestManager.JoinWaiting(this, out waitPos))
                {
                    mover.MoveTo(waitPos);
                    ChangeState(GuestState.Waiting);
                }
                else
                    ChangeState(GuestState.Exit);

                return;
            }
        }

        if (mover.HasArrived())
        {
            mover.StopAndDisable();
            transform.position = myChair.sitPoint.position;
            transform.rotation = myChair.sitPoint.rotation;

            // 바로 WaitOrder로 넘기지 않고, 앉기 애니메이션 실행
            // SitDown 애니메이션 완료 후 WaitOrder 진입
            guestAnim.PlaySitDown();

            // SitDown 완료 콜백에서 상태 전환
            // 기존 OnSitDownFinished에 이미 바인딩되어 있지만,
            // 여기서 직접 전환하는 게 더 명확
            guestAnim.OnSitDownFinished += OnSitDownComplete;
        }

    }

    private void OnSitDownComplete()
    {
        // 중복 호출 방지
        guestAnim.OnSitDownFinished -= OnSitDownComplete;
        ChangeState(GuestState.WaitOrder);
    }

    private void HandleWaitingState()
    {
        myChair = SeatManager.Instance.FindEmptyChair();

        if (myChair != null)
        {
           myChair.Reserve(this);

           // 웨이팅 목록에서 빠지기
           guestManager.LeaveWaiting(this);

           // 빈자리로 이동
           mover.MoveTo(myChair.sitPoint.position);
           isEnterPathSet = true;

           ChangeState(GuestState.Enter);
        }
    }
    
    protected virtual void HandleWaitOrderState()
    {
        // VeryBad 퇴장 중이면 아무것도 안 함
        if (isAngryExiting) return;
        
        stateTimer += Time.deltaTime;

        if (!myOrder.HasOrdered && stateTimer >= menuChooseTime)
        {
            int randomIndex = UnityEngine.Random.Range(0, menuBoard.recipes.Count);
            RecipeData choseRecipe = menuBoard.recipes[randomIndex];

            myOrder.recipe = choseRecipe;
            myOrder.orderedFood = choseRecipe.result;
            myOrder.isServed = false;

            OnGuestReadyToOrder?.Invoke(this);
        }

        if (stateTimer >= orderMaxWaitLimit && CurrentMood == GuestMood.Bad)
        {
            SetMood(GuestMood.VeryBad);
            TriggerAngryExit(-1);
        }
        else if (stateTimer >= orderMinWaitLimit && CurrentMood == GuestMood.Neutral)
        {
            SetMood(GuestMood.Bad);
        }
    }

    protected virtual void HandleWaitFoodState()
    {
        if (isAngryExiting) return;
        
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodMaxWaitLimit)
        {
            SendReward(GuestRewardCalculator.Penalty(-2));
            TriggerAngryExit(-2);
        }
        else if (stateTimer >= foodMidWaitLimit && CurrentMood == GuestMood.Bad)
            SetMood(GuestMood.VeryBad);
        else if (stateTimer >= foodMinWaitLimit && CurrentMood == GuestMood.Neutral)
            SetMood(GuestMood.Bad);
    }
    
    protected virtual void HandleEatState()
    {
        if (isAngryExiting) return;
        
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodEatLimit)
        {
            // 밥 다 먹음 → HandDown 실행
            isPlayingHandDown = true;
            guestAnim.PlayHandDown();
            // HandDown 완료 → BindAnimCallbacks에서 Pay/StandUp 처리
            ChangeState(GuestState.Pay);
        }
    }

    protected virtual void HandlePayState()
    {
        // HandDown/StandUp 애니메이션 진행 중이면 대기
        if (isPlayingHandDown || isPlayingStandUp) return;
        
        if (stateTimer == 0f)
        {
            if (foodQuality == FoodQuality.Good && CurrentMood == GuestMood.Neutral)
                SetMood(GuestMood.VeryGood);
            else if (foodQuality == FoodQuality.Fine && CurrentMood == GuestMood.Neutral)
                SetMood(GuestMood.Good);

            OnGuestReadyToPay?.Invoke(this);

            int basePrice = (myOrder != null && myOrder.orderedFood != null) ? myOrder.orderedFood.price : 0;
            var result = GuestRewardCalculator.Calculate(basePrice, CurrentMood, rewardData);
            SendReward(result);

            OnGuestPaid?.Invoke();

            if (currentFoodModel != null)
            {
                Destroy(currentFoodModel);
                currentFoodModel = null;
            }
        }

        stateTimer += Time.deltaTime;
    }

    private void HandleExitState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isExitPathSet && exitDoor != null)
        {
            OnGuestExited?.Invoke();
            
            if (myChair != null)
            {
                myChair.Empty();
                myChair = null;
            }
            
            if (exitDoor != null)
            {
                mover.EnableAgent();
                mover.MoveTo(exitDoor.position);
                isExitPathSet = true;
            }
        }

        if (isExitPathSet && mover.HasArrived(2.0f))
        {
            PoolManager.Instance.Return(myApperance.ToString(), gameObject);
        }
    }

    // ── VeryBad 분노 퇴장 통합 메서드 ──
    private void TriggerAngryExit(int penaltyLevel)
    {
        if (isAngryExiting) return;

        isAngryExiting = true;
        SetMood(GuestMood.VeryBad);
        SendReward(GuestRewardCalculator.Penalty(penaltyLevel));

        // Angry 모션 실행 → 완료 후 StandUp → 완료 후 Exit
        guestAnim.PlayAngry();
    }

    // 주문 대기 시 플레이어 상호작용 클래스
    public void TakeOrder()
    {
        if (isAngryExiting) return;
        if (currentState != GuestState.WaitOrder || myOrder == null) return;
        if (!myOrder.HasOrdered) return; // 메뉴 안 골랐으면 무시
        if (currentState != GuestState.WaitOrder) return; // 이미 주문을 접수했으면 무시

        isOrderTaken = true;
        OnOrderPlaced?.Invoke(this, myOrder);
        ChangeState(GuestState.WaitFood);
    }

     // 음식 대기 시 플레이어 상호작용 클래스
    public void ReceiveFood(FoodData deliveredFood, FoodQuality quality)
    {
        if (isAngryExiting) return;
        if (currentState != GuestState.WaitFood) return;

        if (myChair != null && myChair.myFoodSpot != null && deliveredFood.foodPrefab != null)
        {
            currentFoodModel = Instantiate(deliveredFood.foodPrefab, myChair.myFoodSpot.position, myChair.myFoodSpot.rotation);
            currentFoodModel.transform.SetParent(myChair.myFoodSpot);
        }

        foodQuality = quality;
        Debug.Log(deliveredFood.name);
        if (myOrder.orderedFood == deliveredFood && !myOrder.isServed)
        {
            myOrder.isServed = true;
            OnOrderServed?.Invoke(this);
            
            if (quality == FoodQuality.Fine)
                SetMood(GuestMood.Good);
            else if (quality == FoodQuality.Good)
                SetMood(GuestMood.VeryGood);

            guestAnim.PlayHandUp();
            ChangeState(GuestState.Eat);
        }
        else
        {
            if (CurrentMood == GuestMood.VeryBad)
            {
                TriggerAngryExit(-2);
                return;
            }

            SetMood(GuestMood.Bad);

            if (quality == FoodQuality.Fine || quality == FoodQuality.Good)
                SetMood(GuestMood.Neutral);

            guestAnim.PlayHandUp();
            ChangeState(GuestState.Eat);
        }
    }

    
    // 기분 변경 + 이벤트 발사를 한 번에
    protected void SetMood(GuestMood newMood)
    {
        CurrentMood = newMood;
        OnGuestMoodChanged?.Invoke(this, CurrentMood);
    }

    protected void SendReward(GuestRewardCalculator.RewardResult result)
    {
        if (result.money > 0)
            EconomyManager.Instance.AddMoney(result.money);

        if (result.reputation != 0)
            EconomyManager.Instance.AddFame(result.reputation);;
    }

    /// <summary>
    ///  영업 끝나면 강제로 exit
    /// </summary>
    public void ForceExit()
    {
        if (!gameObject.activeSelf)
            return;

        // 자리 정리만 (이건 안 하면 버그남)
        if (myChair != null)
        {
            myChair.Empty();
            myChair = null;
        }

        // 웨이팅 제거
        guestManager?.LeaveWaiting(this);

        // 이동/상태/이벤트 전부 무시하고 즉시 반환
        PoolManager.Instance.Return(myApperance.ToString(), gameObject);
    }
}
