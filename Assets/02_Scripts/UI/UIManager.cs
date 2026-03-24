using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;
    [SerializeField] private GameObject questPopupObj;
    [SerializeField] private Button _questButton;
    private bool _isQuestPopupOpen = false;

    protected override void Awake()
    {
        base.Awake();
        _canvas = FindFirstObjectByType<Canvas>();
        _questButton.onClick.AddListener(OpenQuestPopup);
    }
    
    public void OpenDialogPopup(string characterName, string dialog)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(characterName, dialog);
    }
    
    public void OpenQuestPopup()
    {
        questPopupObj.SetActive(!_isQuestPopupOpen);
        _isQuestPopupOpen = !_isQuestPopupOpen;
        if (questPopupObj != null)
            questPopupObj.GetComponent<QuestPopupController>().Show();
    }

    public void AddQuest(Quest quest)
    {
        questPopupObj.GetComponent<QuestPopupController>().AddQuest(quest);
    }
    
    public void EnableGuestOrderUI(/*GuestBase guestBase*/)
    {
        // Instantiate 말고 enabled로 바꿀수도
        var guestOrderUIObject = Instantiate(dialogPopupPrefab/* guestBase*/);
        
        guestOrderUIObject.GetComponent<OrderIconController>().Showt();
    }
}