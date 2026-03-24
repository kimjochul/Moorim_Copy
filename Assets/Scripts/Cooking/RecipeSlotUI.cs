using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSlotUI : MonoBehaviour
{
    // 필요한 UI 컴포넌트들
    [Header("UI")]
    [SerializeField] private Image recipeIcon;
    [SerializeField] private TMP_Text recipeName;

    [Header("Button")]
    [SerializeField] private Button slotBtn;

    // 기억해야 할 데이터
    private Recipe myRecipe; // 내 주머니에 넣을 레시피 데이터
    private CookingController controller;

    // 메소드1 : 초기화할 함수
    // 매개변수 : 나에게 부여된 레시피 데이터, 보고할 컨트롤러의 연락처
    public void Init(Recipe recipe, CookingController ctrl)
    {
        // 매개변수로 전달 받은 데이터를 내 주머니에 저장
        myRecipe = recipe;
        controller = ctrl;

        RecipeData data = myRecipe.Data; // 알맹이 꺼내기

        // 데이터에서 받은 레시피 이름으로 변경
        recipeIcon.sprite = data.icon;
        recipeName.text = data.recipeName;

        // 버튼 클릭 시 메소드 2를 연결
        slotBtn.onClick.AddListener(OnSlotClicked);
    }

    // 메소드2 : 버튼이 눌렸을 때 실행될 함수
    private void OnSlotClicked()
    {
        controller.OnRecipeSelected(myRecipe);
        Debug.Log($"{myRecipe.Data.recipeName} 선택됨");
    }
}
