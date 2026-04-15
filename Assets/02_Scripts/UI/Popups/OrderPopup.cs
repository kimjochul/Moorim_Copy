using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private Image foodIcon;
    [SerializeField] private TMP_Text foodNameText;
    
    private GuestBase guest;
    public GuestBase Gest => guest;

    public void Init(GuestBase _guest, GuestOrderData order)
    {
        guest = _guest;

        if (order.orderedFood != null)
        {
            foodIcon.sprite = order.orderedFood.icon;
            foodNameText.text = order.orderedFood.name;
        }
        
    }
}