using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;

    protected override void Awake()
    {
        base.Awake();
        _canvas = FindFirstObjectByType<Canvas>();
    }
    
    public void OpenDialogPopup(string characterName, string dialog)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(characterName, dialog);
    }

    
}