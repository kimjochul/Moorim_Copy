using UnityEngine;

public interface IItem
{
    int ID { get;}
    
    Inventory Inven { get; }
    GameObject Obj { get; }
    string ItemName { get; }
    Sprite Icon { get; }
    
    int InventoryCapacity { get; }

    void Get();
    void Use();
    
    // 인연 아이템의 특수 버프 효과 용 값, 액션 혹은 함수 필요
}
