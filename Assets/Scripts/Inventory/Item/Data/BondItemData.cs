using UnityEngine;

[CreateAssetMenu(fileName = "BondItemData", menuName = "Scriptable Objects/BondItemData")]
public class BondItemData : ItemData
{
    // 인연 아이템은 특수 아이템 판정이여서 버프 되는 곳(EX. economymanager.cs, eventmanager.cs, orderserve.cs 등등)에서 각각 버프값이 적용되게 해야할지 고민 중
    public int bondWithWhom; // 누구랑 인연이 있는 아이템인지 캐릭터의 id로 구분
    public int buffAmount; // 버프 되는 양, 값
}
