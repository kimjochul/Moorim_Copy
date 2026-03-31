using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class OrdersPopupController : PopupController
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _orderPopupPrefab;
    [SerializeField] private Transform _contentTransform;
    
    [SerializeField] private GameObject _orderNotificationObj;
    [SerializeField] private GameObject _orderPopupBackgroundObj;
    
    Dictionary<GuestBase, RuntimeOrder>  _activeOrders = new Dictionary<GuestBase, RuntimeOrder>();

    void Start()
    {
        GuestBase.OnOrderPlaced += AddOrder;
        _closeButton.onClick.AddListener(OnClickCancelButton);
    }
    
    public void Showt()
    {
        base.Show();
        
        UpdateNotification(false);
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide(() => _orderPopupBackgroundObj.SetActive(false));
    }

    
    public void AddOrder(GuestBase guestBase, RuntimeOrder order)
    {
        if (_activeOrders[guestBase] == order)
            return;
        
        var newOrderPopup = Instantiate(_orderPopupPrefab, _contentTransform);
        Transform pos =  newOrderPopup.transform;
        newOrderPopup.transform.localPosition = new Vector3(pos.position.x, pos.position.y, 0);
        var orderPopup =  newOrderPopup.GetComponent<OrderPopup>();
        orderPopup.Init(guestBase, order);
        _activeOrders[guestBase] = order;

        // 창이 켜져 있지 않으면 알림 켜기
        UpdateNotification(!gameObject.activeSelf);
    }

    void UpdateNotification(bool isActive)
    {
        _orderNotificationObj.SetActive(isActive);
    }
}


