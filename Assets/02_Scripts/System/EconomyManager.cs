using System;
using UnityEngine;

/// <summary>
/// 객잔의 경제 관련 기능을 할 때 사용하는 클래스
/// </summary>
public class EconomyManager : Singleton<EconomyManager>
{
    // 가게의 재정 관련 행동을 할 클래스
    [SerializeField] private DayManager dayManager;
    [SerializeField] private RestaurantEconomy restaurantEconomy;
    [SerializeField] private GuestResultTracker guestResultTracker;
    public RestaurantEconomy RestaurantEconomy => restaurantEconomy;

    private int _todayMoneyIncome;
    public int TodayMoneyIncome => _todayMoneyIncome;
    private int _todayFameIncome;
    public int TodayFameIncome => _todayFameIncome;
    private int _todayCustomerCount;
    public int TodayCustomerCount => _todayCustomerCount;

    public event Action RestaurantEconomyChanged;
    public event Action TodayEconomyChanged;
    
    protected override void Awake()
    {
        base.Awake();
        if (dayManager == null)
            dayManager = DayManager.Instance;
    }
    
    void OnEnable()
    {
        Debug.Log("Eco Enable");
        dayManager.OnDayStart += ResetDay;
        
        dayManager.OnDayEnd += TotalEarningToday;
        dayManager.OnDayEnd += TodayResult;

        GuestManager.OnGuestServed += OnCustomerServed;
        dayManager.OnDayEnd += guestResultTracker.EndDay;
    }

    void OnDisable()
    {
        Debug.Log("Eco Disable");
        dayManager.OnDayStart -= ResetDay;
        
        dayManager.OnDayEnd -= TotalEarningToday;
        dayManager.OnDayEnd -= TodayResult;

        GuestManager.OnGuestServed -= OnCustomerServed;
        dayManager.OnDayEnd -= guestResultTracker.EndDay;
    }
    
    /// <summary>
    /// 주간 영업 중에 금일 수입을 업데이트하기 위한 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount)
    {
        _todayMoneyIncome += amount;
        RestaurantEconomy.Money += amount;
        
        TodayEconomyChanged?.Invoke();
        RestaurantEconomyChanged?.Invoke();
    }
    
    /// <summary>
    /// 주간 영업 중에 금일 명성 치 수입을 업데이트하기 위한 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddFame(int amount)
    {
        _todayFameIncome += amount;
        RestaurantEconomy.Fame += amount;
        
        TodayEconomyChanged?.Invoke();
        RestaurantEconomyChanged?.Invoke();
    }
    
    /// <summary>
    /// CustomerManager 액션 받는 장소
    /// </summary>
    void OnCustomerServed()
    {
        _todayCustomerCount++;
        TodayEconomyChanged?.Invoke();
    }
    
    /// <summary>
    /// 영업 시작 시, 금일 영업 이득을 0으로 초기화하기 위한 함수
    /// </summary>
    public void ResetDay()
    {
        _todayMoneyIncome = 0;
        _todayFameIncome = 0;
        _todayCustomerCount = 0;
        TodayEconomyChanged?.Invoke();
    }

    /// <summary>
    /// 하루 영업 종료 후, 가게 수익을 가게 재정 상태에 업데이트 
    /// </summary>
    public void TotalEarningToday()
    {
        RestaurantEconomyChanged?.Invoke();
    }

    /// <summary>
    /// 증가량, 결과 재정, 왔던 손님 수 등등 전체 종합
    /// </summary>
    public void TodayResult()
    {
        Debug.Log($"금일 수입 : {_todayMoneyIncome}, 금일 명성 증가량 : {_todayFameIncome}");
        Debug.Log($"처리한 손님 수 : {_todayCustomerCount}");
        Debug.Log($"결과 재정 상태: {RestaurantEconomy.Money}, 결과 명성 수치 : {RestaurantEconomy.Fame}");
    }
    
    /// <summary>
    /// 돈 사용 가능 여부
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CanSpend(int amount)
    {
        return restaurantEconomy.Money >= amount;
    }

    /// <summary>
    /// 돈 사용 함수
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool SpendMoney(int amount)
    {
        if (!CanSpend(amount))
            return false;

        restaurantEconomy.Money -= amount;

        RestaurantEconomyChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// 빚 상환 시 호출 함수
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RepayDebt(int amount)
    {
        if (!CanSpend(amount))
            return false;
        
        restaurantEconomy.Debt -= amount;

        RestaurantEconomyChanged?.Invoke();
        return true;
    }
}
