using UnityEngine;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class CustomerManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    
    void Awake()
    {
        dayManager.OnDayStart += StartSpawn;
        dayManager.OnDayEnd += StopSpawn;
    }
    
    /// <summary>
    /// 손님 스폰 시작 함수
    /// </summary>
    public void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnCustomer), 1f, 5f); // 고객 생성, 랜덤 요소 추가 필요
    }

    void SpawnCustomer()
    {
        Debug.Log("손님 생성");
    }
    
    /// <summary>
    /// 손님 스폰 종료 함수
    /// </summary>
    public void StopSpawn()
    {
        CancelInvoke();
    }
}
