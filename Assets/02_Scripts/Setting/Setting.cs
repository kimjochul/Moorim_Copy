using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject objSetting;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnExit;

    private UnityAction onShow;
    private UnityAction onHide;

    void Start()
    {
        EventInit();
    }
    private void EventInit()
    {
        this.btnClose.onClick.AddListener(() => UIManager.Instance.ActiveSetting(false));
        this.btnExit.onClick.AddListener(OnClickExit);

    }
    public void Show()
    {
        this.onShow?.Invoke();
        this.objSetting.SetActive(true);
    }
    public void Hide()
    {
        this.onHide?.Invoke();
        this.objSetting.SetActive(false);
    }
    
    private void OnClickExit()
    {
        UIManager.Instance.CreateOkPopup(
            "게임을 종료하시겠습니까?",
            onOk: () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            },
            onCancel: () => { },
            useCancel: true
        );
    }
}
