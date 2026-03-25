using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPopupController : PopupController
{
    [SerializeField] private GameObject _questPopupPrefab;
    [SerializeField] private Transform _contentTransform;
    
    [SerializeField] private GameObject _questNotificationObj;
    
    Dictionary<QuestPopup, Quest>  _questPopups = new Dictionary<QuestPopup, Quest>();

    void Start()
    {
        // TODO: GuestManager의 action에 UpdateQuestPopups()붙이기
        
        // TODO: 퀘스트 리스트 받아오는 거로 바꾸기
        MakeAllQuestPopups(new List<Quest>());
    }
    public void Show(List<Quest> activeQuests)
    {
        base.Show();
        UpdateNotification(false);
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }

    // QuestManager의 activeQuests 리스트 받아오기
    public void MakeAllQuestPopups(List<Quest> activeQuests)
    {
        foreach (var quest in activeQuests)
        {
            AddQuestPopup(quest);
        }
    }
    
    // 해당 퀘스트를 퀘스트 목록 UI에 추가 (QuestManager의 액션에 붙일 예정)
    public void AddQuestPopup(Quest quest)
    {
        var newQuestPopup = Instantiate(_questPopupPrefab, _contentTransform);
        Transform pos =  newQuestPopup.transform;
        newQuestPopup.transform.localPosition = new Vector3(pos.position.x, pos.position.y, 0);
        var questPopup =  newQuestPopup.GetComponent<QuestPopup>();
        questPopup.Init(quest);
        _questPopups.Add(questPopup, quest);
        
        UpdateNotification(!gameObject.activeSelf);
    }

    // 해당 퀘스트를 퀘스트 목록 UI에서 제거 (QuestManager의 액션에 붙일 예정)
    public void DeleteQuestPopup(Quest quest)
    {
        foreach (var popup in _questPopups)
        {
            if (popup.Value == quest)
            {
                _questPopups.Remove(popup.Key);
                Destroy(popup.Key.gameObject);
                return;
            }
        }
        Debug.Log("<color=red>QuestPopupController: DestroyQuestPopup 실행되었지만 해당 questPopup을 찾을 수 없음</color>");
    }

    void UpdateNotification(bool isActive)
    {
        _questNotificationObj.SetActive(isActive);
    }

    void OnDestroy()
    {
        // TODO: 액션에서 빼기
    }
}
