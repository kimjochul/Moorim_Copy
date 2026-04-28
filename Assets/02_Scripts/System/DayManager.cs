using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

[Serializable]
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

/// <summary>
/// 주간 운영에서 시간, 날짜를 관리하는 클래스
/// </summary>
public class DayManager : Singleton<DayManager>
{
    // 날짜 계산 관련 매니저
    
    [SerializeField] private float dayLength = 600f;
    [SerializeField] private float gameStartHour = 9f;
    [SerializeField] private float gameEndHour = 18f;
    
    private List<INPCSchedule> npcs = new List<INPCSchedule>();

    private float _totalGameMinutes;
    private float _timer = 0f;
    
    [SerializeField] private DayData dayData;
    public DayData DayData {  get => dayData; private set => dayData = value; }

    public bool IsRunning { get; private set; }
    
    public event Action OnDayStart; // 각 매니저에서 가게 운영이 시작될 때의 동작들을 여기에 등록
    public event Action OnDayEnd; // 각 매니저에서 가게 운영이 종료될 때의 동작들을 여기에 등록
    public event Action<int, int> OnTimeChanged;
    
    private int _hour;
    private int _prevHour = -1;
    private int _minute;
    private int _prevMinute = -1;
    
    private bool _isFirstDayStart = true;

    protected override void Awake()
    {
        base.Awake();
        
        _totalGameMinutes = (gameEndHour - gameStartHour) * 60f;
        
        if(!SaveManager.Instance.isLoadedGame)
            InitDay();
    }
    
    public void RegisterNPC(INPCSchedule npc)
    {
        if (!npcs.Contains(npc))
            npcs.Add(npc);
    }

    public void UnregisterNPC(INPCSchedule npc)
    {
        if (npcs.Contains(npc))
            npcs.Remove(npc);
    }
    
    private void InitDay()
    {
        DayData.day = 1;
        DayData.dayOfWeek = DayOfWeek.Monday;
    }
    
    /// <summary>
    /// 주간 영업이 시작될 때 호출되는 함수. OnDayStart Action을 이용해 다른 매니저들도 영업이 시작될 때 맞는 행동을 한다.
    /// </summary>
    public void DayStart()
    {
        Debug.Log("Day Start");
        _timer = 0f;
        IsRunning = true;
        
        _hour = (int)gameStartHour;
        _minute = 0;
        _prevHour = -1;
        _prevMinute = -1;

        SaveManager.Instance.SaveGame();
        
        if (_isFirstDayStart)
        {
            _isFirstDayStart = false;
        }
        else
        {
            UpdateDayData();
        }
        OnDayStart?.Invoke();
    }

    public void RemoteDayStart()
    {
        DayStart();
    }
    
    void Update()
    {
        if (!IsRunning) return;
        
        _timer += Time.deltaTime;

        float t = Mathf.Clamp01(_timer / dayLength);
        float currentGameMinutes = t * _totalGameMinutes;

        float currentHour = gameStartHour + currentGameMinutes / 60f;
        _hour = Mathf.FloorToInt(currentHour);
        _minute = Mathf.FloorToInt((currentHour - _hour) * 60f);

        if (_hour != _prevHour || _minute != _prevMinute)
        {
            _prevHour = _hour;
            _prevMinute = _minute;

            OnTimeChanged?.Invoke(_hour, _minute);
        }
        
        //Debug.Log($"{_hour:00}:{_minute:00}");
        
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
        SaveManager.Instance.SaveGame();
        OnDayEnd?.Invoke();
    }

    /// <summary>
    /// 날짜 데이터 업데이트 함수
    /// </summary>
    private void UpdateDayData()
    {
        dayData.day++;
        dayData.dayOfWeek = (DayOfWeek)(((int)dayData.dayOfWeek + 1) % 7);
        Debug.Log($"오늘 날짜 {dayData.day}일, {dayData.dayOfWeek}");
    }
    
    public bool HasAnyAvailableNPC()
    {
        foreach (var npc in npcs)
        {
            if (npc.IsAvailableDay())
                return true;
        }
        return false;
    }
    
    public DaySaveData GetSaveData()
    {
        return new DaySaveData
        {
            day = DayData.day,
            dayOfWeek = (int)DayData.dayOfWeek
        };
    }
    
    public void LoadFromData(DaySaveData data)
    {
        DayData.day = data.day;
        DayData.dayOfWeek = (DayOfWeek)data.dayOfWeek;
        
        _isFirstDayStart = false;
    }
    
    public void ResetData()
    {
        if (dayData == null)
        {
            Debug.LogError("DayData 없음");
            return;
        }

        dayData.day = 1;
        dayData.dayOfWeek = DayOfWeek.Monday;

        _timer = 0f;
        IsRunning = false;

        _isFirstDayStart = true;

        Debug.Log("DayManager Reset 완료");
    }
}
