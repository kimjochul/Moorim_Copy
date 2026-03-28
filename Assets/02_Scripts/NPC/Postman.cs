using UnityEngine;

public class Postman : MonoBehaviour, IInteractable
{
    [SerializeField] private DayManager dayManager;
    
    [SerializeField] private GameObject postmanView;

    private void Start()
    {
        dayManager.OnDayEnd += DailyQuestCreate; 
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        postmanView.SetActive(IsAvailableDay());
    }
    
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return IsAvailableDay();
    }

    public void Interact(Inventory inventory = null)
    {
        if (!IsAvailableDay()) return;
        // UI 창 호출 눌렀을 때
        // UI 호출 코드
    }
    
    /// <summary>
    /// 랜덤 퀘스트 이전거 삭제 후 3개 재발행 (매주 화 목요일 마다 호출)
    /// </summary>
    private void DailyQuestCreate()
    {
        if (IsAvailableDay())
        {
            // 1. 기존 랜덤 퀘스트 제거
            QuestManager.Instance.ClearRandomQuests();

            // 2. 새 랜덤 퀘스트 3개 생성
            for (int i = 0; i < 3; i++)
            {
                QuestManager.Instance.NotifyListener(new QuestEvent
                {
                    type = QuestEventType.GenerateQuest 
                }, QuestType.PostMan);
            }
        }
    }
    
    private bool IsAvailableDay()
    {
        return dayManager.DayData.dayOfWeek == DayOfWeek.Tuesday ||
               dayManager.DayData.dayOfWeek == DayOfWeek.Thursday;
    }

    public Vector3 GetInteractPosition()
    {
        return default;
    }
}
