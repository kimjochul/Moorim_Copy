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
    
    Dictionary<OrderPopup, Order>  _activeOrders = new Dictionary<OrderPopup, Order>();

    void Start()
    {
        // 주문 리스트 받아오기
        MakeAllOrderPopups(new  List<Order>());
    }
    
    public void Showt()
    {
        base.Show();
        _closeButton.onClick.AddListener(OnClickCancelButton);
        UpdateNotification(false);
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }

    // 퀘스트 추가될 때마다 QuestManager의 activeQuests 리스트 받아오기
    public void MakeAllOrderPopups(List<Order> activeOrders)
    {
        foreach (var order in activeOrders)
        {
            AddOrder(order);
        }
    }
    public void AddOrder(Order order)
    {
        if (_activeOrders.ContainsValue(order))
            return;
        var newOrderPopup = Instantiate(_orderPopupPrefab, _contentTransform);
        Transform pos =  newOrderPopup.transform;
        newOrderPopup.transform.localPosition = new Vector3(pos.position.x, pos.position.y, 0);
        var orderPopup =  newOrderPopup.GetComponent<OrderPopup>();
        orderPopup.Init(order);
        _activeOrders[orderPopup] = order;

        // 창이 켜져 있지 않으면 알림 켜기
        UpdateNotification(!gameObject.activeSelf);
    }

    void UpdateNotification(bool isActive)
    {
        _orderNotificationObj.SetActive(isActive);
    }
}


