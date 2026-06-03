using System;
using System.Collections.Generic;
using UnityEngine;

// guest가 빈자리를 찾을 때 seatmanager를 통해 빈자리 여부를 확인 하고 길을 찾음
public class SeatManager : Singleton<SeatManager>
{
    private List<Chair> allChairs = new List<Chair>(); // 식당 내 모든 의자
    private HashSet<Chair> emptyChairSet = new HashSet<Chair>(); // 빈자리 검색,추가,제거
    private List<Chair> emptyChairList = new List<Chair>(); // 랜덤 뽑기용 (HashSet은 인덱스 접근 불가)
    
    public event Action<Chair> OnSeatFreed; // 빈자리가 생겼을 때 발생할 이벤트

    // 의자가 처음 설치되거나 게임이 켜질 때 명단에 등록
    public void RegisterChair(Chair chair)
    {
        if (!allChairs.Contains(chair))
            allChairs.Add(chair);

        if (chair.IsEmpty && !emptyChairSet.Contains(chair))
        {
            emptyChairSet.Add(chair);
            emptyChairList.Add(chair);
        }
    }
    
    // 손님이 빈자리 찾는 메소드
    public Chair FindEmptyChair()
    {
        if (emptyChairList.Count == 0) // 빈자리가 없으면 null 반환
            return null; 

        int randomIndex = UnityEngine.Random.Range(0, emptyChairList.Count);

        // 랜덤하게 뽑힌 의자 반환
        return emptyChairList[randomIndex];        
    }

    // 누군가 자리를 찜했을 때 > 빈자리 리스트에서 뺌
    public void ReserveChair(Chair chair)
    {
        if (emptyChairSet.Remove(chair)) // Remove가 성공하면 true 반환
            emptyChairList.Remove(chair);
    }

    // 손님이 밥 다 먹고 나가면 -> 빈자리 리스트에 추가
    public void FreeChair(Chair chair)
    {
        if (chair.IsEmpty && emptyChairSet.Add(chair)) // Add가 성공하면 true 반환 (중복이면 false)
        {
            emptyChairList.Add(chair);
            OnSeatFreed?.Invoke(chair);
        }


    }
}
