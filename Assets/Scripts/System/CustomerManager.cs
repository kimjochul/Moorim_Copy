using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // Customer 생성 및 관리 매니저
    [SerializeField] private DayManager dayManager;
    
    void Awake()
    {
        dayManager.OnDayStart += StartSpawn;
        dayManager.OnDayEnd += StopSpawn;
    }
    
    public void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnCustomer), 1f, 5f); // 고객 생성, 랜덤 요소 추가 필요
    }

    void SpawnCustomer()
    {
        Debug.Log("손님 생성");
    }
    
    public void StopSpawn()
    {
        CancelInvoke();
    }
}
