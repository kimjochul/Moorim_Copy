using UnityEngine;
using static Constants;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] private DayManager dayManager;
    
    [SerializeField] private GameObject merchantView;
    [SerializeField] private MerchantUI merchantUI;
 
    // ── IInteractable 구현 ──
 
    // 상인은 클릭해서 상호작용
    public InteractMode Mode => InteractMode.ClickToInteract;
 
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
    
    public bool CanInteract(PlayerType playerType)
    {
        return IsAvailableDay();
    }
 
    public void Interact(PlayerController player)
    {
        if (!IsAvailableDay())
        {
            player.EndInteract();
            return;
        }
 
        // UI 창 호출
        // UI 호출 코드
        // UI 닫을 때 player.EndInteract() 호출 필요
    }
 
    public void OnInteractEnd(PlayerController player)
    {
        // ClickToInteract — 트리거 퇴장으로 호출되지 않음
    }
 
    /// <summary>
    /// 랜덤 퀘스트 이전거 삭제 후 3개 재발행 (매주 월 수 금요일 마다 호출)
    /// </summary>
    private void DailyQuestCreate()
    {
        if (IsAvailableDay())
        {
            QuestManager.Instance.ClearRandomQuests();
 
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
