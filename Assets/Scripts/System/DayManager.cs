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
    
    // 가게 운영 시작 버튼이 눌리면 이 함수 호출되도록
    public void DayStart()
    {
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

    public void DayEnd()
    {
        IsRunning = false;
        UpdateDayData();
        OnDayEnd?.Invoke();
    }

    private void UpdateDayData()
    {
        dayData.day++;
        dayData.dayOfWeek = (DayOfWeek)(((int)dayData.dayOfWeek + 1) % 7);
    }
}
