using UnityEngine;

public interface IItem
{
    Inventory Inven { get; }
    GameObject Obj { get; }
    string ItemName { get; }
    Sprite Icon { get; }
    
    int InventoryCapacity { get; }

    void Get();
    void Use();
}
