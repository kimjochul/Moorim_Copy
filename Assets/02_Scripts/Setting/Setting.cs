using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject objSetting;
    [SerializeField] private Button btnClose;

    private UnityAction onShow;
    private UnityAction onHide;

    void Start()
    {
        if (this.objSetting != null)
        {
            this.objSetting.SetActive(false);
        }
        EventInit();
    }
    private void EventInit()
    {
        this.btnClose.onClick.AddListener(Hide);
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
}
