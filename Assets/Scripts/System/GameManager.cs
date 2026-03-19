using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 다른 매니저들을 관리하고 게임에서 플레이 흐름 제어하는 클래스
    [SerializeField] private DayManager dayManager;
    // [SerializeField] private EventManager eventManager;
    // [SerializeField] private EconomyManager economyManager;
    // [SerializeField] private CustomerManager customerManager;

    private void Start()
    {
        dayManager.DayStart();
    }

    public void StartDayButton()
    {
        dayManager.DayStart();
    }
    
    // void Awake()
    // {
    //     dayManager.OnDayStart += HandleDayStart;
    //     dayManager.OnDayEnd += HandleDayEnd;
    // }
    //
    // void HandleDayStart()
    // {
    //     Debug.Log("하루 시작");
    // }
    //
    // void HandleDayEnd()
    // {
    //     Debug.Log($"오늘 번 돈: {economyManager.RestaurantEconomy.Money}, 오늘 얻은 명성 {economyManager.RestaurantEconomy.Fame}");
    //     
    // }
}
