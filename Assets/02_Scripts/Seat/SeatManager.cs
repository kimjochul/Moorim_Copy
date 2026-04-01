using System.Collections.Generic;
using UnityEngine;

// guest가 빈자리를 찾을 때 seatmanager를 통해 빈자리 여부를 확인 하고 길을 찾음
public class SeatManager : Singleton<SeatManager>
{
    private List<Chair> allChairs = new List<Chair>(); // 식당 내 모든 의자
    private List<Chair> emptyChairs = new List<Chair>(); // 빈 의자

    // 의자가 처음 설치되거나 게임이 켜질 때 명단에 등록
    public void RegisterChair(Chair chair)
    {
        if (!allChairs.Contains(chair))
            allChairs.Add(chair);

        Debug.Log($"[SeatManager] {chair.gameObject.name} 등록 시도 중... / 의자설치됨:{chair.isBuilt} / 테이블찾음:{chair.table != null} / 테이블설치됨:{(chair.table != null ? chair.table.isBuilt.ToString() : "모름")} / 최종 IsEmpty: {chair.IsEmpty}");

        if (chair.IsEmpty && !emptyChairs.Contains(chair))
            emptyChairs.Add(chair);
    }
    
    // 손님이 빈자리 찾는 메소드
    public Chair FindEmptyChair()
    {
        Debug.Log($"[SeatManager] 손님이 자리를 찾습니다! 현재 전체 의자: {allChairs.Count}개 / 빈자리 장부: {emptyChairs.Count}개");
        
        if (emptyChairs.Count == 0) // 빈자리가 없으면 null 반환
            return null; 

        int randomIndex = UnityEngine.Random.Range(0, emptyChairs.Count);

        // 랜덤하게 뽑힌 의자 반환
        return emptyChairs[randomIndex];        
    }

    // 누군가 자리를 찜했을 때 > 빈자리 리스트에서 뺌
    public void ReserveChair(Chair chair)
    {
        if (emptyChairs.Contains(chair))
            emptyChairs.Remove(chair);
    }

    // 손님이 밥 다 먹고 나가면 -> 빈자리 리스트에 추가
    public void FreeChair(Chair chair)
    {
        if (chair.IsEmpty && !emptyChairs.Contains(chair))
            emptyChairs.Add(chair);
    }
}
