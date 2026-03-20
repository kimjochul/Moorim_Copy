using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 데이터를 받아 퀘스트를 생성해서 버튼을 누를 시 버튼에 해당하는 퀘스트를 받게 하고
/// 퀘스트 클리어 창에 퀘스트 클리어에 대한 클리어 바를 만드는 클래스
/// 게임 구조에 따라 변경될 가능성이 큼
/// 게임 데이터와 게임 수락 바를 인게임 중에 생성하는 함수 추가 필요
/// </summary>
public class QuestManager : Singleton<QuestManager>, ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    
    [SerializeField] private Button[] questButtons;
    [SerializeField] private GameObject[] questClearButtons;
    [SerializeField] private Transform questClearUI;
    [SerializeField] private QuestData[] datas;
    
    [SerializeField] private Inventory inventory;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < questButtons.Length; i++)
        {
            int j = i;
            questButtons[i].onClick.AddListener(()=>SetButton(j));
        }
    }

    /// <summary>
    /// 퀘스트 창에서 퀘스트를 받을 때, 퀘스트 데이터를 index(아마 id로 대체될 듯)를 통해 받고
    /// 퀘스트 클리어 창에 퀘스트 클리어 바를 생성하는 함수
    /// </summary>
    /// <param name="index"></param>
    private void SetButton(int index)
    {
        Quest quest = new Quest(datas[index]); // 퀘스트 데이터의 id를 통해서 접근하도록 변경하는게 좋을 듯 
        questButtons[index].gameObject.SetActive(false);
        
        GameObject clearobj = Instantiate(questClearButtons[index], questClearUI);
        clearobj.GetComponent<QuestClearBar>().Init(quest, inventory);
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
    /// 퀘스트 진행 여부를 QuestEvent를 받았을 때, 지금 추적 중인 모든 퀘스트에게 공지하여 본인이 해당이 되는지 알리는 함수
    /// </summary>
    /// <param name="questEvent"></param>
    public void NotifyListener(QuestEvent questEvent)
    {
        for (int i = observers.Count - 1; i >= 0; i--)
            observers[i].Notify(questEvent);
    }
}
