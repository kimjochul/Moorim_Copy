using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;

    private void Awake()
    {
        _canvas = FindFirstObjectByType<Canvas>();
    }
    
    /// <summary>
    /// 대화 팝업 열기
    /// </summary>
    /// <param name="recipes">주문할 레시피들과 수량</param>
    public void OpenDialogPopup(Dictionary<Recipe, int> recipes)
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show(recipes);
    }

    
}