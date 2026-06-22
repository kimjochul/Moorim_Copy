using UnityEngine;
using static Constants; // IngredientType, InventoryType 등을 사용하기 위해


// 돼지고기 : 0
// 닭고기 : 2
// 밀가루 : 3
// 쌀 : 4
// 두부 : 5
// 생강 : 6
// 설탕 : 7
// 고추가루 : 8
// 상추 : 9 (8이 중복되서 9로 수정함)_
public class TestingredientAdd : MonoBehaviour
{
    
    // 유니티 인스펙터 버튼 OnClick()에 연결할 public 함수입니다.
    // 매개변수 ingredientId: 각 버튼에서 넘겨줄 재료의 고유 ID
    public void CreateSpecificItem(int ingredientId)
    {
        // 1. 아이템 팩토리를 통해 요청받은 ID의 아이템을 1개 생성합니다.
        IItem item = ItemFactory.Instance.CreateItem(ingredientId);

        if (item == null || item.Data == null)
        {
            Debug.LogError($"ID {ingredientId} 찾을 수 없음");
            return;
        }

        if (item.Data is IngredientData ingredientData)
        {
            if (ingredientData.type == IngredientType.Meats)
            {
                InventoryManager.Instance.GetInventory(InventoryType.IngredientsButMeatOnly).GetItem(item, 10);
            }
            else
            {
                InventoryManager.Instance.GetInventory(InventoryType.Ingredients).GetItem(item, 10);
            }
        }
        else
        {
            Debug.LogWarning("IngredientData인 ID가 아님.");
        }
    }
}