using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class OkPopup : PopupController
{
    [SerializeField]
    private TextMeshProUGUI txtMessage;
    [SerializeField]
    private Button btnOk;
    
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
}
