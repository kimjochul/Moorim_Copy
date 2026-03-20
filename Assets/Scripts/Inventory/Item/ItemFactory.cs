using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;

    public Item CreateItem(int itemID)
    {
        ItemData itemData = itemDatabase.Get(itemID);
        return new Item(itemData);
    }
}
