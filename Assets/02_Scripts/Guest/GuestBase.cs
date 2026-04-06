using System;
using UnityEngine;
using static Constants;

// 부모 클래스
public abstract class GuestBase : MonoBehaviour
{
    [Header("기본 대기 시간 (자식들이 변경 가능)")]
    [SerializeField] protected float orderWaitLimit = 20f;
    [SerializeField] protected float foodMinWaitLimit = 30f;
    [SerializeField] protected float foodMidWaitLimit = 60f;
    [SerializeField] protected float foodMaxWaitLimit = 90f;
    [SerializeField] protected float foodEatLimit = 15f;
    [SerializeField] protected float menuChooseTime = 3f;

    [Header("레시피 리스트")]
    [SerializeField] private RecipeList menuBoard;

    [Header("이동(A*) 설정")]
    private GuestMover mover;
    public Transform exitDoor;
    public Chair myChair;

    private GuestManager guestManager;

    // 손님 런타임 정보
    public int individualId;
    public Constants.GuestAppearance myApperance;
    public RuntimeOrder myOrder = new RuntimeOrder();

    protected float stateTimer = 0f;
    protected int basePrise;
    private bool isEnterPathSet = false;
    private bool isExitPathSet = false;
    private bool isOrderTaken = false;

    private GameObject currentFoodModel;
    
    public GuestState currentState { get; private set; }
    protected GuestMood currentMood = GuestMood.Neutral;
    // 결제 시 정산할 퀄리티 및 꼼수 방지용
    private FoodQuality FoodQuality = FoodQuality.Normal;

    // Action
    public event Action<GuestBase, GuestState> OnGuestStateChanged; // 상태 변경 알림용
    public event Action<GuestBase, GuestMood> OnGuestMoodChanged; // 상태 변경 알림용
    public event Action<GuestBase> OnGuestReadyToOrder; // 메뉴를 골랐을 때용
    public event Action<GuestBase, RuntimeOrder> OnOrderPlaced; // UI 주문 리스트용
    public event Action OnGuestServedAndPaid; // 스포너용

    // 풀에서 꺼낼 때 호출. 재사용 시 이전 생애의 모든 상태를 깨끗이 초기화해 버그 방지
    public virtual void ResetState()
    {
        if (guestManager != null)
        guestManager.LeaveWaiting(this);
        
        // FSM 플래그 — 이게 안 되면 Enter/Exit에서 경로를 안 잡음
        isEnterPathSet = false;
        isExitPathSet = false;
        isOrderTaken = false;

        // 타이머 & 상태값
        stateTimer = 0f;
        currentMood = GuestMood.Neutral;
        FoodQuality = FoodQuality.Normal;
        basePrise = 0;

        // 주문 데이터
        if (myOrder == null)
            myOrder = new RuntimeOrder();
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

        mover.ResetAgent();

        // 이벤트 구독 전부 해제
        // 안 하면 이전 생애의 리스너가 남아서 이벤트가 중복 호출됨
        OnGuestStateChanged = null;
        OnGuestReadyToOrder = null;
        OnOrderPlaced = null;
        OnGuestServedAndPaid = null;
        OnGuestMoodChanged = null;
    }
    
    // 객체가 생성될 때 매니저에서 딱 한 번 호출해줄 초기화 메소드
    public virtual void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        individualId = id;
        myApperance = appearance;
        exitDoor = exit;
        guestManager = manager;
        
        if (myOrder == null)
            myOrder = new RuntimeOrder();
        else
            myOrder.Clear();

        gameObject.SetActive(true);

        mover.EnableAgent();
        
        ChangeState(GuestState.Enter);
    }

    void Awake()
    {
        mover = GetComponent<GuestMover>();
    }

    void Update()
    {
        GuestFSM();
    }

    protected void ChangeState(Constants.GuestState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        Debug.Log($"{currentState}로 상태 변경");
        OnGuestStateChanged?.Invoke(this, currentState); // UI에 상태 변경 알림
    }

    private void GuestFSM()
    {
        switch(currentState)
        {
            case GuestState.Enter:
                HandleEnterState();
                break;
            case GuestState.Waiting:
                HandleWaitingState();
                break;
            case GuestState.WaitOrder:
                HandleWaitOrderState();
                break;
            case GuestState.WaitFood:
                HandleWaitFoodState();
                break;
            case GuestState.Eat:
                HandleEatState();
                break;
            case GuestState.Pay:
                HandlePayState();
                break;
            case GuestState.Exit:
                HandleExitState();
                break;
        }
    }

    // 입장
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

               ChangeState(GuestState.WaitOrder);
            }
            
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
    
    // 착석 후 주문 대기 (메뉴 고르고 플레이어 기다림)
    protected virtual void HandleWaitOrderState()
    {
        stateTimer += Time.deltaTime;

        if (!myOrder.HasOrdered && stateTimer >= menuChooseTime)
        {
            Debug.Log($"[{individualId}]이 메뉴를 고르는 중입니다.");
            
            int randomIndex = UnityEngine.Random.Range(0, menuBoard.recipes.Count);
            RecipeData choseRecipe = menuBoard.recipes[randomIndex];

            Debug.Log($"[{individualId}]가 {choseRecipe}를 골랐습니다.");

            myOrder.orderedFood = choseRecipe.result;
            myOrder.isServed = false;

            OnGuestReadyToOrder?.Invoke(this); // UI에게 주문 대기 아이콘 띄우라 하기
        }

        if (stateTimer >= orderWaitLimit)
        {
            currentMood = GuestMood.Bad;
            // TO DO : 아이콘 변화
            // TO DO : 명성도 하락 기능
            ChangeState(GuestState.Exit);
            Debug.Log("주문 대기 시간 초과! 손님의 기분이 매우 나빠져 가게를 나갑니다!");

        }
    }

    // 플레이어 상호작용 클래스
    public void TakeOrder()
    {
        if (currentState != GuestState.WaitOrder || myOrder == null)
            return;

        // 이미 주문을 접수했으면 무시
        if (currentState != GuestState.WaitOrder)
            return;

        isOrderTaken = true;
        OnOrderPlaced?.Invoke(this, myOrder); // 주문 리스트에 데이터 전송
        ChangeState(GuestState.WaitFood);
    }

    // 음식 대기
    protected virtual void HandleWaitFoodState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodMaxWaitLimit)
        {
            // TO DO : 아이콘 변화
            ChangeState(Constants.GuestState.Exit);
            Debug.Log("음식 대기 1분 30초 경과! 손님이 화를 내며 가게를 나갑니다!");
            
        }
        else if (stateTimer >= foodMidWaitLimit && currentMood == GuestMood.Bad)
        {
            currentMood = GuestMood.VeryBad;
            // TO DO : 아이콘 변화
            Debug.Log("음식 대기 60초 경과! 손님의 기분이 매우 나빠졌습니다!");
        }
        else if (stateTimer >= foodMinWaitLimit && currentMood == GuestMood.Neutral)
        {
            currentMood = GuestMood.Bad;
            // TO DO : 아이콘 변화
            Debug.Log("음식 대기 30초 경과! 손님의 기분이 나빠졌습니다!");
        }
    }
    
    // 플레이어 상호작용 클래스
    public void ReceiveFood(FoodData deliveredFood, FoodQuality quality)
    {
        if (currentState != GuestState.WaitFood)
            return;

        if (myOrder.orderedFood == deliveredFood && !myOrder.isServed)
        {
            myOrder.isServed = true;
            FoodQuality = quality;

            if (myChair != null && myChair.myFoodSpot != null)
            {
                currentFoodModel = Instantiate(deliveredFood.foodPrefab, myChair.myFoodSpot.position, myChair.myFoodSpot.rotation);
                currentFoodModel.transform.SetParent(myChair.myFoodSpot);
            }

            ChangeState(GuestState.Eat);
        }
        else
        {
            currentMood = GuestMood.VeryBad;
            ChangeState(GuestState.Eat);
        }
    }
    
    protected virtual void HandleEatState()
    {
        // TO DO : 애니메이션 변경
        stateTimer += Time.deltaTime;

        if (stateTimer >= foodEatLimit)
        {
            // TO DO : 애니메이션 변경
            ChangeState(GuestState.Pay);
        }
    }

    protected virtual void HandlePayState()
    {
        if (FoodQuality == FoodQuality.Good && currentMood == GuestMood.Neutral) 
            currentMood = GuestMood.VeryGood; 
        else if (FoodQuality == FoodQuality.Fine && currentMood == GuestMood.Neutral) 
            currentMood = GuestMood.Good; 

        int finalPrice = CalculateFinalPrice();
        
        Debug.Log($"[{individualId}] 식사 완료! 기분: {currentMood}, 최종 결제 금액: {finalPrice}원");

        OnGuestServedAndPaid?.Invoke(); 
        
        // TO DO : 애니메이션 변경?
        if (currentFoodModel != null)
        {
            Destroy(currentFoodModel);
            currentFoodModel = null;
        }
        // TO DO : 아이콘 변경?
        // TO DO : 손님 기분 체크
        // TO DO : 손님 기분에 따라 보상 차등
        // TO DO : 돈 지불 -> 자동으로 하루 매출금액에 합산
        ChangeState(Constants.GuestState.Exit);
    }

    protected virtual int CalculateFinalPrice()
    {
        basePrise = 0;

        if (myOrder != null && myOrder.orderedFood != null)
            basePrise = myOrder.orderedFood.price;

        float moodMultiplier = 1.0f;

        switch (currentMood)
        {
            case Constants.GuestMood.VeryBad: 
                moodMultiplier = 0.5f;
                break; 
            case Constants.GuestMood.Bad: 
                moodMultiplier = 0.8f; 
                break;
            case Constants.GuestMood.Neutral: 
                moodMultiplier = 1.0f; 
                break;
            case Constants.GuestMood.Good: 
                moodMultiplier = 1.2f; 
                break;
            case Constants.GuestMood.VeryGood: 
                moodMultiplier = 1.5f;
                break; 
        }
        
        return Mathf.RoundToInt(basePrise * moodMultiplier);
    }

    private void HandleExitState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isExitPathSet && exitDoor != null)
        {
            if (myChair != null)
            {
                myChair.Empty();
                myChair = null;
            }
            
            mover.MoveTo(exitDoor.position);
            isExitPathSet = true;
        }

        if (isExitPathSet && mover.HasArrived(2.0f))
            PoolManager.Instance.Return(myApperance.ToString(), gameObject);
    }


}
