using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] private DayManager dayManager;
    
    [SerializeField] private GameObject merchantView;
    [SerializeField] private MerchantUI merchantUI;

    private void Start()
    {
        dayManager.OnDayEnd += DailyQuestCreate; 
        merchantUI.Init(this);
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        merchantView.SetActive(IsAvailableDay());
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
    /// 랜덤 퀘스트 이전거 삭제 후 3개 재발행 (매주 월 수 금요일 마다 호출)
    /// </summary>
    private void DailyQuestCreate()
    {
        if (IsAvailableDay())
        {
            // 기존 랜덤 퀘스트 제거 TODO: 이거 퀘스트 지우는 시점은 다시 정해야함
            QuestManager.Instance.ClearRandomQuests();

            // 새 랜덤 퀘스트 3개 생성
            for (int i = 0; i < 3; i++)
            {
                QuestManager.Instance.NotifyListener(new QuestEvent
                {
                    type = QuestEventType.GenerateQuest 
                }, QuestType.Merchant);
            }
        }
    }

    private bool IsAvailableDay()
    {
        return dayManager.DayData.dayOfWeek == DayOfWeek.Monday ||
               dayManager.DayData.dayOfWeek == DayOfWeek.Wednesday || 
               dayManager.DayData.dayOfWeek == DayOfWeek.Friday;
    }
    
    public Vector3 GetInteractPosition()
    {
        return default;
    }
}
