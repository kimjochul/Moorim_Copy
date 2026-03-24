using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingIngredientSlotUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private GameObject countWrap;
    [SerializeField] private TMP_Text ingredientcount;

    // 기억해야 할 데이터
    private IngredientData targetIngredient; // 내가 요구하는 재료 데이터
    private int requiredAmount; // 목표 필요 개수
    private int currentAmount; // 현재 들어온 개수

    // 메소드1 : 초기 세팅 함수
    // 매개변수: 내가 담당할 재료 데이터, 필요 개수
    public void Init (IngredientData ingredient, int amount)
    {
        targetIngredient = ingredient;
        requiredAmount = amount;
        currentAmount = 0;

        ingredientIcon.sprite = ingredient.icon;
        ingredientIcon.color = new Color(1, 1, 1, 0.5f);

        countWrap.gameObject.SetActive(true);
        ingredientcount.text = $"{currentAmount} / {requiredAmount}";

        gameObject.SetActive(true);
    }

    // 메소드2 : 재료가 들어왔을 때 불리는 함수
    // 매개변수: 들어온 개수
    public void AddIngredient(int amount)
    {
        // 현재 갯수를 올리고 텍스트 갱신
        currentAmount += amount;
        ingredientcount.text = $"{currentAmount} / {requiredAmount}";

        if (currentAmount >= requiredAmount)
        {
            ingredientIcon.color = new Color(1, 1, 1, 1);
        }
    }

    // 슬롯이 가지고 있는 목표 재료를 외부에서 꺼내는 용도
    public IngredientData GetTargetIngredient() => targetIngredient;

    public void ClearSlot()
    {
        targetIngredient = null;
        ingredientIcon.sprite = null;
        ingredientIcon.color = new Color (1, 1, 1, 0);
        countWrap.SetActive(false);
        ingredientcount.text = "";
    }
}
