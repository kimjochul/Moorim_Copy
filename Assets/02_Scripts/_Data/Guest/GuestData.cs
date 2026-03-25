using UnityEngine;

[CreateAssetMenu(fileName = "GuestData", menuName = "Scriptable Objects/GuestData")]
public class GuestData : ScriptableObject
{
    public int id;
    public string guestName;
    public GameObject guestPrefab;
    public Sprite icon;
    public Constants.GuestGender guestGender;
    public Order order;
    
    // 대사 등
    [TextArea(3, 10)]
    public string description;
}
