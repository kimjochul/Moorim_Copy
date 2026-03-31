using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderIconController : MonoBehaviour
{
    private Button _button;
    public void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnOrderIconClicked);
    }

    private void OnOrderIconClicked()
    {
        GameObject guestObj = transform.parent.gameObject;
        GuestBase guestBase = guestObj.GetComponent<GuestBase>();
        
        guestBase.TakeOrder();
    }
}
