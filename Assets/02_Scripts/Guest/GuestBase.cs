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
    [SerializeField] private GuestRewardData rewardData;

    protected GuestMover mover;
    private GuestManager guestManager;
    private GuestBubbleUI bubbleUI;
    protected GuestAnimation guestAnim;
    
    protected RecipeUnlockManager recipeUnlockManager;

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

    // 애니메이션 진행 플래그
    private bool isAngryExiting = false;
    public bool IsAngryExiting => isAngryExiting;
    protected bool isPlayingHandDown = false;
    protected bool isPlayingStandUp = false;
    private bool hasShownPayUI = false;
    private bool hasProcessedPayment = false;

    // 음식 모델
    private GameObject currentFoodModel;

    // 상태 & 기분
    public GuestState currentState { get; private set; }
    public GuestMood CurrentMood { get; protected set; } = GuestMood.Neutral;
    private FoodQuality foodQuality = FoodQuality.Normal;

    // Action 이벤트
    public event Action<GuestBase, GuestState> OnGuestStateChanged;
    public event Action<GuestBase, GuestMood> OnGuestMoodChanged;
    public event Action<GuestBase> OnGuestReadyToOrder;
    public event Action<GuestBase, GuestOrderData> OnOrderPlaced;
    public event Action<GuestBase> OnOrderServed;
    public event Action<GuestBase> OnGuestReadyToPay;
    public event Action OnGuestPaid;
    public event Action OnGuestExited;
    public event Action<GuestBase> OnGuestAngryExited;
    public event Action<GuestBase, GuestOrderData> OnOrderChanged;

    void Awake()
    {
        mover = GetComponent<GuestMover>();
        bubbleUI = GetComponentInChildren<GuestBubbleUI>();
        recipeUnlockManager = RecipeUnlockManager.Instance;
    }

    // 풀에서 꺼낼 때 호출
    public virtual void ResetState()
    {
        if (mover == null)
            mover = GetComponent<GuestMover>();
        if (bubbleUI == null)
            bubbleUI = GetComponentInChildren<GuestBubbleUI>();
        if (guestManager != null)
            guestManager.LeaveWaiting(this);

        // FSM 플래그
        isEnterPathSet = false;
        isExitPathSet = false;
        isOrderTaken = false;

        // 애니메이션 플래그
        isAngryExiting = false;
        isPlayingHandDown = false;
        isPlayingStandUp = false;
        hasShownPayUI = false;
        hasProcessedPayment = false;

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

        // 의자 반환
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

        // 이벤트 구독 해제
        OnGuestStateChanged = null;
        OnGuestMoodChanged = null;
        OnGuestReadyToOrder = null;
        OnOrderPlaced = null;
        OnGuestReadyToPay = null;
        OnGuestPaid = null;
        OnGuestExited = null;
        OnOrderServed = null;
        Gate.OnGateStateChanged -= HandleGateStateChanged;
        
        recipeUnlockManager = RecipeUnlockManager.Instance;
    }

    // 객체 생성 시 한 번 호출
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

        Gate.OnGateStateChanged += HandleGateStateChanged;
        ChangeState(GuestState.Enter);
    }

    private void BindAnimCallbacks()
    {
        guestAnim.OnHandUpFinished += () => { };

        guestAnim.OnHandDownFinished += () =>
        {
            isPlayingHandDown = false;
        };

        guestAnim.OnStandUpFinished += () =>
        {
            isPlayingStandUp = false;
            ChangeState(GuestState.Exit);
        };

        guestAnim.OnAngryFinished += () =>
        {
            if (isAngryExiting)
            {
                isPlayingStandUp = true;
                guestAnim.PlayStandUp();
            }
        };
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
        switch (currentState)
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

    private void HandleEnterState()
    {
        if (!isEnterPathSet)
        {
            myChair = SeatManager.Instance.FindEmptyChair();

            if (Gate.isOpen && myChair != null)
            {
                myChair.Reserve(this);
                mover.MoveTo(myChair.sitPoint.position);
                isEnterPathSet = true;
            }
            else
            {
                Vector3 waitPos;
                if (guestManager.JoinWaiting(this, out waitPos))
                {
                    mover.MoveTo(waitPos);
                    ChangeState(GuestState.Waiting);
                }
                else
                {
                    ChangeState(GuestState.Exit);
                }

                return;
            }
        }

        if (mover.HasArrived())
        {
            mover.StopAndDisable();

            transform.position = myChair.sitPoint.position;
            transform.rotation = myChair.sitPoint.rotation;

            guestAnim.PlaySitDown();
            guestAnim.OnSitDownFinished += OnSitDownComplete;
        }
    }

    private void OnSitDownComplete()
    {
        guestAnim.OnSitDownFinished -= OnSitDownComplete;
        ChangeState(GuestState.WaitOrder);
    }

    private void HandleWaitingState()
    {
    }
    
    public void AssignChairAndEnter(Chair chair)
    {
        myChair = chair;
        myChair.Reserve(this);

        mover.MoveTo(myChair.sitPoint.position);
        
        isEnterPathSet = true;
        
        ChangeState(GuestState.Enter);
    }

    protected virtual void HandleWaitOrderState()
    {
        if (isAngryExiting) return;

        stateTimer += Time.deltaTime;

        if (!myOrder.HasOrdered && stateTimer >= menuChooseTime)
        {
            RecipeData choseRecipe = recipeUnlockManager.RandomUnlockedRecipe();

            myOrder.recipe = choseRecipe;
            myOrder.orderedFood = choseRecipe.result;
            myOrder.isServed = false;

            OnGuestReadyToOrder?.Invoke(this);
        }

        if (stateTimer >= orderMaxWaitLimit && CurrentMood == GuestMood.Bad)
        {
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

        if (stateTimer >= foodMaxWaitLimit && CurrentMood == GuestMood.Bad)
        {
            SetMood(GuestMood.VeryBad);
            TriggerAngryExit(-2);  
        }
        else if (stateTimer >= foodMinWaitLimit && CurrentMood == GuestMood.Neutral)
            SetMood(GuestMood.Bad);
    }

    protected virtual void HandleEatState()
    {
        if (isAngryExiting) return;

        stateTimer += Time.deltaTime;

        if (stateTimer >= foodEatLimit)
        {
            isPlayingHandDown = true;
            guestAnim.PlayHandDown();
            ChangeState(GuestState.Pay);
        }
    }

    protected virtual void HandlePayState()
    {
        if (isAngryExiting) return;

        if (isPlayingHandDown)
            return;

        if (!hasShownPayUI)
        {
            if (foodQuality == FoodQuality.Good && CurrentMood == GuestMood.Neutral)
                SetMood(GuestMood.VeryGood);
            else if (foodQuality == FoodQuality.Fine && CurrentMood == GuestMood.Neutral)
                SetMood(GuestMood.Good);

            OnGuestReadyToPay?.Invoke(this);
            hasShownPayUI = true;
            stateTimer = 0f;
            return;
        }

        stateTimer += Time.deltaTime;

        if (stateTimer >= payWaitTime && !hasProcessedPayment)
        {
            hasProcessedPayment = true;

            int basePrice = (myOrder != null && myOrder.orderedFood != null)
                ? myOrder.orderedFood.price : 0;
            var result = GuestRewardCalculator.Calculate(basePrice, CurrentMood, rewardData);
            SendReward(result);

            OnGuestPaid?.Invoke();

            if (currentFoodModel != null)
            {
                Destroy(currentFoodModel);
                currentFoodModel = null;
            }

            isPlayingStandUp = true;
            guestAnim.PlayStandUp();
        }
    }

    protected virtual void HandleExitState()
    {
        if (!isExitPathSet)
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
            if (Vector3.Distance(transform.position, exitDoor.position) <= 3.0f)
            {
                PoolManager.Instance.Return(myApperance.ToString(), gameObject);
            }
        }
    }

    public void TakeOrder()
    {
        if (isAngryExiting) return;
        if (currentState != GuestState.WaitOrder || myOrder == null) return;
        if (!myOrder.HasOrdered) return;

        isOrderTaken = true;
        OnOrderPlaced?.Invoke(this, myOrder);
        ChangeState(GuestState.WaitFood);
    }

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

    private void TriggerAngryExit(int penaltyLevel)
    {
        if (isAngryExiting) return;

        isAngryExiting = true;
        SetMood(GuestMood.VeryBad);
        SendReward(GuestRewardCalculator.Penalty(penaltyLevel));
        
        OnGuestAngryExited?.Invoke(this);
        
        guestAnim.PlayAngry();
    }

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
            EconomyManager.Instance.AddFame(result.reputation);
    }

    public void ForceExit()
    {
        if (!gameObject.activeSelf)
            return;

        if (myChair != null)
        {
            myChair.Empty();
            myChair = null;
        }

        guestManager?.LeaveWaiting(this);

        PoolManager.Instance.Return(myApperance.ToString(), gameObject);
    }
    
    protected void InvokeOrderChanged()
    {
        OnOrderChanged?.Invoke(this, myOrder);
    }
    
    public void TimeSetForTutorial()
    {
        orderMinWaitLimit = 99999f;
        orderMaxWaitLimit = 99999f;
        foodMinWaitLimit = 99999f;
        foodMidWaitLimit = 99999f;
        foodMaxWaitLimit = 99999f;
    }
    
    private void HandleGateStateChanged(bool isGateOpen)
    {
        if (mover != null)
        {
            mover.RecalculatePath();
        }
    }
}