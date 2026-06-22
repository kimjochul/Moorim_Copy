using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class OkPopup : PopupController
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnCancel;
    
    public void Init(string message)
    {
        this.txtMessage.text = message;
    }
    public void ButtonOk(UnityAction action)
    {
        this.btnOk.onClick.AddListener(() =>
        {
            action?.Invoke();
            Destroy(this.gameObject);
        });
    }

    public void ButtonCancel(UnityAction action)
    {
        this.btnCancel.onClick.AddListener(() =>
        {
            action?.Invoke();
            Destroy(this.gameObject);
        });
    }
}
