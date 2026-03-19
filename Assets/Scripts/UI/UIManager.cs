using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    // 캔버스
    private Canvas _canvas;
    [SerializeField] private GameObject dialogPopupPrefab;
    
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬에서 Canvas 참조 가져오기
        _canvas = FindFirstObjectByType<Canvas>();
    }
    
    // 대화 팝업 열기
    public void OpenDialogPopup()
    {
        var dialogPopupObject = Instantiate(dialogPopupPrefab, _canvas.transform);
        dialogPopupObject.GetComponent<DialogPopupController>().Show();
    }

    
}