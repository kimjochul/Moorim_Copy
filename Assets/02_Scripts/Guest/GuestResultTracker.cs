using System;
using UnityEngine;
using static Constants;

public class GuestResultTracker : MonoBehaviour
{
    // 하루 집계
    private GuestResultData dailyResult = new GuestResultData();

    // 누적 집계
    private GuestResultData totalResult = new GuestResultData();

    // 하루 마감 시 외부에 알림
    public event Action<GuestResultData, GuestResultData> OnDayResultReady;

    public void RecordGuest(GuestMood mood)
    {
        dailyResult.AddResult(mood);
        totalResult.AddResult(mood);
    }

    public void EndDay()
    {
        OnDayResultReady?.Invoke(dailyResult, totalResult);
        dailyResult.Reset();
    }

    public GuestResultData GetDailyResult() => dailyResult;
    public GuestResultData GetTotalResult() => totalResult;
}
