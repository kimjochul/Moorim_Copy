using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 퀘스트 데이터를 받아 퀘스트를 생성해서 버튼을 누를 시 버튼에 해당하는 퀘스트를 받게 하고
/// 퀘스트 클리어 창에 퀘스트 클리어에 대한 클리어 바를 만드는 클래스
/// 게임 구조에 따라 변경될 가능성이 큼
/// 게임 데이터와 게임 수락 바를 인게임 중에 생성하는 함수 추가 필요
/// </summary>
public class QuestManager : Singleton<QuestManager>, ISubject
{
    [SerializeField] private QuestUIManager questUIManager;
    
    private List<IObserver> observers = new List<IObserver>();
    
    [SerializeField] private QuestGenerator generator;
    
    [SerializeField] private Inventory inventory;
    private List<Quest> activeQuests = new List<Quest>();
    public event Action<Quest> OnActiveQuestAdded; // ActiveQuest 리스트에 새 퀘스트 추가 시 호출 
    public event Action<Quest> OnActiveQuestRemoved; // ActiveQuest 리스트에 퀘스트 제거 시 호출
    private List<Quest> waitingQuests = new List<Quest>();
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        QuestManager.Instance.NotifyListener(new QuestEvent
        {
            type = QuestEventType.GenerateQuest
        });
    }

    /// <summary>
    /// 퀘스트 수락 전 제안
    /// </summary>
    /// <param name="data"></param>
    public void OfferQuest(Quest quest)
    {
        waitingQuests.Add(quest);
        questUIManager.CreateAcceptUI(quest);
    }
    
    /// <summary>
    /// 퀘스트 창에서 퀘스트를 받을 때
    /// 퀘스트 클리어 창에 퀘스트 클리어 바를 생성하는 함수
    /// </summary>
    /// <param name="index"></param>
    public void AcceptQuest(Quest quest)
    {
        if (activeQuests.Exists(q => q.QuestID == quest.QuestID))
            return;

        activeQuests.Add(quest);
        waitingQuests.Remove(quest);
        Instance.AddObserver(quest);
     
        OnActiveQuestAdded?.Invoke(quest);
        
        questUIManager.CreateClearUI(quest, inventory);
    }

    /// <summary>
    /// 퀘스트 완료될 때 호출, 현재 진행하는 activequest 리스트에서 퀘스트 삭제 후 알림 액션 호출
    /// </summary>
    /// <param name="quest"></param>
    public void EraseQuestFromActive(Quest quest)
    {
        if (activeQuests.Exists(q => q.QuestID == quest.QuestID))
            activeQuests.Remove(quest);
        
        OnActiveQuestRemoved?.Invoke(quest);
    }
    
    /// <summary>
    /// 퀘스트를 생성할 때 퀘스트 매니저에서 퀘스트를 추적하기 위해 설정을 하는 함수
    /// </summary>
    /// <param name="observer"></param>
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
        Debug.Log($"퀘스트 {observer.QuestName}를 등록하였습니다.");
    }

    /// <summary>
    ///  퀘스트 완료, 실패 등으로 퀘스트 추적을 그만 두기 위한 함수
    /// </summary>
    /// <param name="observer"></param>
    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
        Debug.Log($"퀘스트 {observer.QuestName}를 삭제하였습니다.");
    }
    
    /// <summary>
    /// 이벤트 타입이 퀘스트생성이면 생성기에게 퀘스트 생성 명령
    /// 퀘스트 진행 여부를 QuestEvent를 받았을 때, 지금 추적 중인 모든 퀘스트에게 공지하여 본인이 해당이 되는지 알리는 함수
    ///
    /// 아래와 같은 호출 방식을 이용하면 랜덤 퀘스트 생성
    ///QuestManager.Instance.NotifyListener(new QuestEvent
    /// {
    ///     type = QuestEventType.GenerateQuest
    /// });
    /// 
    /// </summary>
    /// <param name="questEvent"></param>
    public void NotifyListener(QuestEvent questEvent)
    {
        if (questEvent.type == QuestEventType.GenerateQuest)
        {
            var quest = generator.GenerateRandomQuest();

            if (quest != null)
                OfferQuest(quest);

            return;
        }
        
        for (int i = observers.Count - 1; i >= 0; i--)
            observers[i].Notify(questEvent);
    }
}
