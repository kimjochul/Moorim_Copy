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
    [SerializeField] private float dayLength = 600f;
    private float _timer = 0f;
    
    [SerializeField] private DayData dayData;
    public DayData DayData {  get => dayData; private set => dayData = value; }

    public bool IsRunning { get; private set; }
    
    public event Action OnDayStart;
    public event Action OnDayEnd;

    public void Start()
    {
        DayData = new DayData
        {
            day = 1,
            dayOfWeek = DayOfWeek.Monday
        };
    }

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
