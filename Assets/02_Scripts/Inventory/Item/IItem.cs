using System;
using UnityEngine;

public interface IItem
{
    int ID { get;}
    ItemData Data { get;}
    Inventory Inven { get; }
    string ItemName { get; }
    Sprite Icon { get; }
    
    int InventoryCapacity { get; }
    
    float SpoilTimer { get; set; }
    int FridgeSpoilTimer { get; set; }

    // 인연 아이템의 특수 버프 효과 용 값, 액션 혹은 함수 필요
}
