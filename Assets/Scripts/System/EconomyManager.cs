using System;
using UnityEngine;

[Serializable]
public class RestaurantEconomy
{
    internal int Money;
    internal int Fame;
}

/// <summary>
/// 객잔의 경제 관련 기능을 할 때 사용하는 클래스
/// </summary>
public class EconomyManager : MonoBehaviour
{
    // 가게의 재정 관련 행동을 할 클래스
    [SerializeField] DayManager dayManager;
    
    public RestaurantEconomy RestaurantEconomy { get; private set; }

    private int _todayMoneyIncome;
    private int _todayFameIncome;
    
    void Awake()
    {
        RestaurantEconomy = new RestaurantEconomy();
        
        dayManager.OnDayStart += ResetDay;
        dayManager.OnDayEnd += TotalEarningToday;
    }
    
    /// <summary>
    /// 주간 영업 중에 금일 수입을 업데이트하기 위한 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount)
    {
        _todayMoneyIncome += amount;
    }
    
    /// <summary>
    /// 주간 영업 중에 금일 명성 치 수입을 업데이트하기 위한 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddFame(int amount)
    {
        _todayFameIncome += amount;
    }
    
    /// <summary>
    /// 영업 시작 시, 금일 영업 이득을 0으로 초기화하기 위한 함수
    /// </summary>
    public void ResetDay()
    {
        _todayMoneyIncome = 0;
        _todayFameIncome = 0;
    }

    /// <summary>
    /// 하루 영업 종료 후, 가게 수익을 가게 재정 상태에 업데이트 
    /// </summary>
    public void TotalEarningToday()
    {
        RestaurantEconomy.Money += _todayMoneyIncome;
        RestaurantEconomy.Fame += _todayFameIncome;
    }
}
