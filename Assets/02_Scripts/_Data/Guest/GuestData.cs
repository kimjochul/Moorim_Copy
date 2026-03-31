using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable] // 손님이 쥐고 있을 전체 주문서
public class RuntimeOrder
{
    public RecipeData recipe; // 고른 메뉴
    public bool isServed; // 음식을 받았는지 여부

    // 메뉴가 채워져 있으면 주문 완료 상태로 봄
    public bool HasOrdered => recipe != null;
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
