using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : Singleton<QuestManager>, ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    
    [SerializeField] private Button[] questButtons;
    [SerializeField] private GameObject[] questClearButtons;
    [SerializeField] private Transform questClearUI;
    [SerializeField] private QuestData[] datas;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < questButtons.Length; i++)
        {
            int j = i;
            questButtons[i].onClick.AddListener(()=>SetButton(j));
        }
    }

    private void SetButton(int index)
    {
        Quest quest = new Quest(datas[index]);
        questButtons[index].gameObject.SetActive(false);
        
        GameObject clearobj = Instantiate(questClearButtons[index], questClearUI);
        clearobj.GetComponent<QuestClearBar>().Init(quest);
    }

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
        Debug.Log($"퀘스트 {observer.QuestName}를 등록하였습니다.");
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
        Debug.Log($"퀘스트 {observer.QuestName}를 삭제하였습니다.");
    }

    public void NotifyListener(string questName)
    {
        for (int i = observers.Count - 1; i >= 0; i--)
            observers[i].Notify(questName);
    }
}
