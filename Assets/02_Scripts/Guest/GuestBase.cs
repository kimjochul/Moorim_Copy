using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// 부모 클래스
public abstract class GuestBase : MonoBehaviour 
{
    [Header("기본 대기 시간 (자식들이 변경 가능)")]
    [SerializeField] protected float orderWaitLimit = 20f;
    [SerializeField] protected float foodMinWaitLimit = 30f;
    [SerializeField] protected float foodMidWaitLimit = 60f;
    [SerializeField] protected float foodMaxWaitLimit = 90f;
    [SerializeField] protected float foodEatLimit = 15f;

    [Header("레시피 리스트")]
    [SerializeField] private RecipeList menuBoard;

    [Header("이동(A*) 설정")]
    [SerializeField] private NavMeshAgent agent; // 실제로 움직이는 객체
    public Transform targetTable; 
    public Transform exitDoor;

    // 손님 런타임 정보
    public int individualId;
    public GuestEnums.GuestAppearance myApperance;
    public RuntimeOrder myOrder;

    protected float stateTimer = 0f;
    protected int basePrise;
    private bool isEnterPathSet = false;
    private bool isExitPathSet = false;
    
    protected GuestEnums.GuestState currentState = GuestEnums.GuestState.Enter; 
    protected GuestEnums.GuestMood currentMood = GuestEnums.GuestMood.Neutral;
    // 결제 시 정산할 퀄리티 및 꼼수 방지용 그릇 카운터
    private GuestEnums.FoodQuality FoodQuality = GuestEnums.FoodQuality.Normal;
    private int totalReceivedPlates = 0;

    public static event Action<GuestBase, GuestEnums.GuestState> OnGuestStateChanged; // 상태 변경 알림용
    public static event Action<GuestBase> OnGuestReadyToOrder; // 메뉴를 골랐을 때용
    public static event Action<GuestBase, RuntimeOrder> OnOrderPlaced; // UI 주문 리스트용
    public static event Action OnGuestServedAndPaid; // 스포너용
    
    // 객체가 생성될 때 매니저에서 딱 한 번 호출해줄 초기화 메소드
    public virtual void Init(int id, GuestEnums.GuestAppearance appearance, Transform table, Transform exit)
    {
        individualId = id;
        myApperance = appearance;
        targetTable = table;
        exitDoor = exit;
        
        ChangeState(GuestEnums.GuestState.Enter);
    }
    
    void Update()
    {
        GuestFSM();
    }

    private void ChangeState(GuestEnums.GuestState newState)
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

    
    // 입장
    private void HandleEnterState()
    {
        // 상태 진입 후 딱 한 번만 목적지 찾기
        if (!isEnterPathSet && targetTable != null)
        {
            agent.SetDestination(targetTable.position);
            isEnterPathSet = true;
            Debug.Log($"[{individualId}] 빈 테이블로 이동 시작");
        }

        // 목적지에 도착했는지 검사
        // pathpending : 경로 계산 중인지 여부, remainingDistance : 목적지까지 남은 거리, stoppingDistance: 목표 지점에서 멈추는 거리
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            //TO DO : 애니메이션 변경
            ChangeState(GuestEnums.GuestState.WaitOrder);
        }
    }

    // 착석 후 주문 대기 (메뉴 고르고 플레이어 기다림)
    protected virtual void HandleWaitOrderState()
    {
        stateTimer += Time.deltaTime;

        if (stateTimer <= orderWaitLimit && myOrder.item.Count == 0)
        {
            Debug.Log($"[{individualId}]이 메뉴를 고르는 중입니다.");
            
            int randomIndex = UnityEngine.Random.Range(0, menuBoard.recipes.Count);
            RecipeData choseRecipe = menuBoard.recipes[randomIndex];
            Debug.Log($"[{individualId}]가 {choseRecipe}를 골랐습니다.");

            myOrder.item.Add(new OrderItem { recipe = choseRecipe, requredAmount = 1, servedAmount = 0});
            OnGuestReadyToOrder?.Invoke(this); // UI에게 주문 대기 아이콘 띄우라 하기
        }

        if (stateTimer >= orderWaitLimit)
        {
            currentMood = GuestEnums.GuestMood.Bad;
            // TO DO : 아이콘 변화
            // TO DO : 명성도 하락 기능
            ChangeState(GuestEnums.GuestState.Exit);
            Debug.Log("주문 대기 시간 초과! 손님의 기분이 매우 나빠져 가게를 나갑니다!");

        }
    }

    // 플레이어 상호작용 클래스
    public void TakeOrder()
    {
        if (currentState != GuestEnums.GuestState.WaitOrder || myOrder == null)
            return;

        OnOrderPlaced?.Invoke(this, myOrder); // 주문 리스트에 데이터 전송
        ChangeState(GuestEnums.GuestState.WaitFood);
    }

    // 음식 대기
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

    
    // 플레이어 상호작용 클래스
    public void ReceiveFood(RecipeData deliveredFood, GuestEnums.FoodQuality quality)
    {
        if (currentState != GuestEnums.GuestState.WaitFood)
            return;

        totalReceivedPlates++; 
        
        // 내 주문서 검사
        OrderItem matchingItem = myOrder.item.Find(x => x.recipe == deliveredFood && !x.IsComplete);

        if (matchingItem != null)
            matchingItem.servedAmount++;
        else
        {
            matchingItem.servedAmount++;
            currentMood = GuestEnums.GuestMood.VeryBad;
        }

        int totalRequiredPlates = myOrder.item.Sum(x => x.requredAmount);

        // 주문한 수량만큼 음식을 전달 받았다면
        if (totalReceivedPlates >= totalRequiredPlates)
        {
            FoodQuality = quality;
            ChangeState(GuestEnums.GuestState.Eat);
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
        if (FoodQuality == GuestEnums.FoodQuality.Good && currentMood == GuestEnums.GuestMood.Neutral) 
            currentMood = GuestEnums.GuestMood.VeryGood; 
        else if (FoodQuality == GuestEnums.FoodQuality.Fine && currentMood == GuestEnums.GuestMood.Neutral) 
            currentMood = GuestEnums.GuestMood.Good; 

        int finalPrice = CalculateFinalPrice();
        
        Debug.Log($"[{individualId}] 식사 완료! 기분: {currentMood}, 최종 결제 금액: {finalPrice}원");

        OnGuestServedAndPaid?.Invoke(); 
        
        // TO DO : 애니메이션 변경?
        // TO DO : 아이콘 변경?
        // TO DO : 손님 기분 체크
        // TO DO : 손님 기분에 따라 보상 차등
        // TO DO : 돈 지불 -> 자동으로 하루 매출금액에 합산
        ChangeState(GuestEnums.GuestState.Exit);
    }

    
    protected virtual int CalculateFinalPrice()
    {
        basePrise = 0;

        if (myOrder != null && myOrder.item != null)
        {
            foreach (var i in myOrder.item)
            {
                basePrise += i.recipe.price * i.requredAmount;
            }
        }

        float moodMultiplier = 1.0f;

        switch (currentMood)
        {
            case GuestEnums.GuestMood.VeryBad: 
                moodMultiplier = 0.5f;
                break; 
            case GuestEnums.GuestMood.Bad: 
                moodMultiplier = 0.8f; 
                break;
            case GuestEnums.GuestMood.Neutral: 
                moodMultiplier = 1.0f; 
                break;
            case GuestEnums.GuestMood.Good: 
                moodMultiplier = 1.2f; 
                break;
            case GuestEnums.GuestMood.VeryGood: 
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
            agent.SetDestination(exitDoor.position);
            isExitPathSet = true;
            Debug.Log("손님이 문으로 향해 나갑니다.");
        }

        // 목적지에 도착했는지 검사
        if (isExitPathSet && !agent.pathPending && agent.remainingDistance <= 2.0f)
        {
            Debug.Log("손님이 가게를 벗어났습니다.");
            Destroy(gameObject);
        }
    }
    
}
