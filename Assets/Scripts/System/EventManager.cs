using UnityEngine;

public class EventManager : MonoBehaviour
{
    // 이벤트 관련 매니저
    [SerializeField] DayManager dayManager;
    
    void Awake()
    {
        dayManager.OnDayStart += GenerateEvent;
    }

    private void GenerateEvent()
    {
        
    }
}
