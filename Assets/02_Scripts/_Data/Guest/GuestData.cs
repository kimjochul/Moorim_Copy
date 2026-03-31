using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable] // 인스펙터 노출용 개별 주문서
public class OrderItem
{
    public RecipeData recipe; // 주문한 요리
    public int requredAmount; // 요구 수량
    public int servedAmount; // 플레ㅣ어가 서빙한 수량

    public bool IsComplete => servedAmount >= requredAmount;
}

[System.Serializable] // 손님이 쥐고 있을 전체 주문서
public class RuntimeOrder
{
    public List<OrderItem> item = new List<OrderItem>();

    public bool IsAllOrderComplete()
    {
        return item.All(item => item.IsComplete);
    }
}


[CreateAssetMenu(fileName = "GuestData", menuName = "Scriptable Objects/GuestData")]
public class GuestData : ScriptableObject
{
    public int id;
    public string guestName;
    public GameObject guestPrefab;
    public Sprite icon;
    public Order order;
    
    // 대사 등
    [TextArea(3, 10)]
    public string description;
}
