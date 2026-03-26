using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class OrdersPopupController : PopupController
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _orderPopupPrefab;
    [SerializeField] private Transform _contentTransform;
    List<Order>  _activeOrders = new List<Order>();
    public void Show(List<Order> activeOrders)
    {
        base.Show();
        _closeButton.onClick.AddListener(OnClickCancelButton);
        MakeAllOrderPopups(activeOrders);
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
        if (_activeOrders.Contains(order))
            return;
        var newOrderPopup = Instantiate(_orderPopupPrefab, _contentTransform);
        Transform pos =  newOrderPopup.transform;
        newOrderPopup.transform.localPosition = new Vector3(pos.position.x, pos.position.y, 0);
        var orderPopup =  newOrderPopup.GetComponent<OrderPopup>();
        orderPopup.Init(order);
        _activeOrders.Add(order);
    }
}


