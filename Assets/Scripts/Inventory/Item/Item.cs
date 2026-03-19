using UnityEngine;

public class Item : IItem
{
    public int ID { get; }
    public Inventory Inven { get; }
    public string ItemName { get; }
    public Sprite Icon { get; }
    public int InventoryCapacity { get; }
    
    public ItemData Data { get; private set; }
    public Item(ItemData data)
    {
        Data = data;

        ID = data.id;
        ItemName = data.itemName;
        InventoryCapacity = data.maxCapacity;
        Icon = data.icon;
    }
}
