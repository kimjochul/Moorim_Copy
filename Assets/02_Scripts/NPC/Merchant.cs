using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] private DayManager dayManager;
    
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return false;
    }

    public void Interact(Inventory inventory = null)
    {
        // UI 창 호출 눌렀을 때
        gameObject.SetActive(true);
    }

    private void Start()
    {
        dayManager.OnDayEnd += DailyQuestCreate; 
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 랜덤 퀘스트 이전거 삭제 후 3개 재발행 (매주 월 수 금요일 마다 호출)
    /// </summary>
    private void DailyQuestCreate()
    {
        if (dayManager.DayData.dayOfWeek == DayOfWeek.Monday || dayManager.DayData.dayOfWeek == DayOfWeek.Wednesday || dayManager.DayData.dayOfWeek == DayOfWeek.Friday)
        {
            // 1. 기존 랜덤 퀘스트 제거
            QuestManager.Instance.ClearRandomQuests();

            // 2. 새 랜덤 퀘스트 3개 생성
            for (int i = 0; i < 3; i++)
            {
                QuestManager.Instance.NotifyListener(new QuestEvent
                {
                    type = QuestEventType.GenerateQuest 
                }, QuestType.Merchant);
            }
        }
    }
}
