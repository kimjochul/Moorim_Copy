using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderIconController : MonoBehaviour
{
    [SerializeField] private Button _orderButton;
    GuestBase _guestBase;
    public void Awake()
    {
        _orderButton = GetComponent<Button>();
        _guestBase = GetComponent<GuestBase>();
        _orderButton.onClick.AddListener(OnOrderIconClicked);
    }

    public void EnableOrderIcon()
    {
        _orderButton.gameObject.SetActive(true);
    }

    private void OnOrderIconClicked()
    {
        Debug.Log("<color=yellow>OrderClicked</color>");
        _orderButton.gameObject.SetActive(false);
        
        
        _guestBase.TakeOrder();
    }
}
