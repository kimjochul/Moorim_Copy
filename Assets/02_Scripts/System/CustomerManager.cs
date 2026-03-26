using System;
using UnityEngine;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class CustomerManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;

    public static event Action OnCustomerServed;
    private int customerCount = 0;
    
    void Awake()
    {
        dayManager.OnDayStart += StartSpawn;
        dayManager.OnDayEnd += StopSpawn;
        dayManager.OnDayEnd += ResetCustomerCount;
    }
    
    /// <summary>
    /// 손님 스폰 시작 함수
    /// </summary>
    private void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnCustomer), 1f, 5f); // 고객 생성, 랜덤 요소 추가 필요
    }

    private void SpawnCustomer()
    {
        Debug.Log("손님 생성");
        ServeCustomer();
    }
    
    /// <summary>
    /// 손님을 제대로 서빙까지 마쳐서 성공적으로 대접이 끝날 때 호출(인원 갱신 및 economymanager에 액션 전송)
    /// </summary>
    void ServeCustomer()
    {
        customerCount++;
        OnCustomerServed?.Invoke(); 
    }

    private void ResetCustomerCount()
    {
        customerCount = 0;
    }
    
    /// <summary>
    /// 손님 스폰 종료 함수
    /// </summary>
    private void StopSpawn()
    {
        CancelInvoke();
    }
}
