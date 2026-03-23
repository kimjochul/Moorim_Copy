using System;
using UnityEngine;

public enum DayOfWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

[Serializable]
public class DayData
{
    internal int day;
    internal DayOfWeek dayOfWeek;
}


/// <summary>
/// 주간 운영에서 시간, 날짜를 관리하는 클래스
/// </summary>
public class DayManager : MonoBehaviour
{
    // 날짜 계산 관련 매니저
    
    [SerializeField] private float dayLength = 600f;
    private float _timer = 0f;
    
    [SerializeField] private DayData dayData;
    public DayData DayData {  get => dayData; private set => dayData = value; }

    public bool IsRunning { get; private set; }
    
    public event Action OnDayStart; // 각 매니저에서 가게 운영이 시작될 때의 동작들을 여기에 등록
    public event Action OnDayEnd; // 각 매니저에서 가게 운영이 종료될 때의 동작들을 여기에 등록

    public void Awake()
    {
        DayData = new DayData
        {
            day = 1,
            dayOfWeek = DayOfWeek.Monday
        };
    }
    
    /// <summary>
    /// 주간 영업이 시작될 때 호출되는 함수. OnDayStart Action을 이용해 다른 매니저들도 영업이 시작될 때 맞는 행동을 한다.
    /// </summary>
    public void DayStart()
    {
        Debug.Log("Day Start");
        _timer = 0f;
        IsRunning = true;
        OnDayStart?.Invoke();
    }

    void Update()
    {
        if (!IsRunning) return;
        
        _timer += Time.deltaTime;

        if (_timer >= dayLength)
        {
            DayEnd();
        }
    }
    
    /// <summary>
    /// 주간 영업이 끝날 때 호출되는 함수. OnDayStart Action을 이용해 다른 매니저들도 영업이 끝날 때 맞는 행동을 한다.
    /// </summary>
    public void DayEnd()
    {
        Debug.Log("Day End");
        IsRunning = false;
        UpdateDayData();
        OnDayEnd?.Invoke();
    }

    /// <summary>
    /// 날짜 데이터 업데이트 함수
    /// </summary>
    private void UpdateDayData()
    {
        dayData.day++;
        dayData.dayOfWeek = (DayOfWeek)(((int)dayData.dayOfWeek + 1) % 7);
    }
}
