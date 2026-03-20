using UnityEngine;

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
