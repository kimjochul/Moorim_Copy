using System;
using UnityEngine;

[Serializable]
public class RestaurantEconomy
{
    internal int Money;
    internal int Fame;
}

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
    
    // 메뉴를 판매 할 때마다 혹은 가게 운영 하루 마무리 할 때 하루 수입 정리
    public void AddMoney(int amount)
    {
        _todayMoneyIncome += amount;
    }
    public void AddFame(int amount)
    {
        _todayFameIncome += amount;
    }
    
    public void ResetDay()
    {
        _todayMoneyIncome = 0;
        _todayFameIncome = 0;
    }

    // 하루 영업 종료 후, 가게 수익을 가게 재정 상태에 업데이트 
    public void TotalEarningToday()
    {
        RestaurantEconomy.Money += _todayMoneyIncome;
        RestaurantEconomy.Fame += _todayFameIncome;
    }
}
