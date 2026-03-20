using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSlotUI : MonoBehaviour
{
    [SerializeField] private Image recipeImg;
    [SerializeField] private TextMeshPro recipeName;
    [SerializeField] private Button selectRecipe;
    [SerializeField] private GameObject lockOverlay; // 해금 안 됐을 때 덮어 씌울 자물쇠 이미지

    private Recipe myRecipe;
    private CookingController controller;

    //// 실제 가지고있어야할 데이터들 ////
    // 음식 이미지
    // 음식 이름
    // 음식에 대한 정보
    // 음식을 만들 때 필요한 재료 정보
    // 해금 여부?
    ///////////////////////////////////
    
    // 리스트가 생성될 때 데이터를 주입받을 함수
    private void InitSlot(Recipe recipe, CookingController ctrl, bool isUnlock)
    {
        // UI에 적용되어야할 데이터
        //recipeImg = recipe.icon
        
    }
    

    // 컨트롤러에게 자신이 눌렸다는 걸 알리는 함수

    // 카테고리 필터링을 위해 자기 자신의 데이터를 반환하는 헬퍼 함수
    
}
