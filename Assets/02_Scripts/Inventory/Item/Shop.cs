using UnityEngine;

/// <summary>
/// 상인과 인벤토리, 객잔의 economymanager를 연결해서 아이템 구매하는데 사용할 클래스
/// </summary>
public class Shop : MonoBehaviour
{
    [SerializeField] private ItemFactory itemFactory;
    [SerializeField] private Inventory playerInventory;

    public void Buy(int itemID, int count)
    {
        // 돈 체크는 생략
        Item item = itemFactory.CreateItem(itemID);

        playerInventory.GetItem(item, count);
    }
}
