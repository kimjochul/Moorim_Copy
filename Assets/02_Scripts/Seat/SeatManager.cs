using System.Collections.Generic;
using UnityEngine;

// guest가 빈자리를 찾을 때 seatmanager를 통해 빈자리 여부를 확인 하고 길을 찾음
public class SeatManager : Singleton<SeatManager>
{
    private List<Chair> allChairs = new List<Chair>(); // 식당 내 모든 의자
    private List<Chair> emptyChairs = new List<Chair>(); // 빈 의자

    // 의자가 처음 설치되거나 게임이 켜질 때 명단에 등록
    private void RegisterChair(Chair chair)
    {
        if (!allChairs.Contains(chair))
            allChairs.Add(chair);

        if (chair.IsAvailable && !emptyChairs.Contains(chair))
            emptyChairs.Add(chair);
    }
    
    // 손님이 문에서 가장 가까운 빈자리 찾는 메소드
    public Chair FindEmptyChair()
    {
        // if ()
        foreach(Chair chair in allChairs)
        {
            if (chair.IsAvailable) // 의자가 빈자리일 경우
                return chair;      // 그 의자를 배정
        }

        return null; // 빈자리가 없으면 null 반환
    }
}
