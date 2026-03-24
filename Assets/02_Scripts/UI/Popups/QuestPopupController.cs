using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPopupController : PopupController
{
    [SerializeField] private GameObject _questPopupPrefab;
    [SerializeField] private Transform _contentTransform;
    List<QuestPopup>  _questPopups = new List<QuestPopup>();
    public void Show()
    {
        base.Show();
        // GameObject newQuestPopup = Instantiate(_questPopupPrefab, _contentTransform);

    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }

    public void AddQuest(Quest quest)
    {
        var newQuestPopup = Instantiate(_questPopupPrefab, _contentTransform);
        Transform pos =  newQuestPopup.transform;
        newQuestPopup.transform.localPosition = new Vector3(pos.position.x, pos.position.y, 0);
        var questPopup =  newQuestPopup.GetComponent<QuestPopup>();
        questPopup.Init(quest);
        _questPopups.Add(questPopup);
    }

}
