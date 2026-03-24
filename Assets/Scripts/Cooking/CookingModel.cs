using System.Collections.Generic;
using UnityEngine;

public class CookingModel
{
    // 기억해야할 데이터
    // 현재 목표로 삼은 레시피(지금 만들고있는 요리)
    public Recipe CurrentRecipe { get; private set; }
    // 재료 ID, 진행 상태를 추적하는 딕셔너리
    public Dictionary<int, int> InsertedIngredients { get; private set; } = new Dictionary<int, int>();

    // 메소드1. 새로운 요리 시작을 준비하는 함수
    // 매개변수 : 선택된 레시피
    public void SetTargetRecipe(Recipe recipe)
    {
        CurrentRecipe = recipe;
        InsertedIngredients.Clear();

        // 레시피에 필요한 재료 목록을 가져오고 각 재료를 0개 상태로 등록
        foreach (var req in recipe.Data.ingredients)
        {
            InsertedIngredients[req.ingredient.id] = 0;
        }
    }

    // 메소드2. 인베토리에서 온 재료가 맞는지 검사하는 함수
    // 매개변수 : 투입된 재료 id, 갯수
    public int TryAddIngredient(int ingredientId, int amount)
    {
        if (CurrentRecipe == null)
            return 0;

        // 필요 없는 재료면 0개 투입
        if (!InsertedIngredients.ContainsKey(ingredientId))
            return 0;

        // 현재 투입량과 목표치 가져오기
        int current = InsertedIngredients[ingredientId];
        int required = CurrentRecipe.Data.ingredients.Find(x => x.ingredient.id == ingredientId).amount;

        // 남은 빈 공간 계산
        int spaceLeft = required - current;

        // 이미 갯수 충족이 된 경우 0개 투입
        if (spaceLeft <= 0)
            return 0;

        // 투입량 결정을 위한 변수
        int actualAdd = 0;

        if (amount > spaceLeft)
            actualAdd = spaceLeft; // 넣으려는 갯수가 빈 공간보다 많으면 빈 공간만큼만 맞춰서 넣음
        else    
            actualAdd = amount; // 안 넘치면 넣으려는 갯수 그대로 넣음

        // 한도 제한해서 넣고 성공 보고
        InsertedIngredients[ingredientId] += actualAdd;
        return actualAdd;
    }

    // 메소드3. 조리 가능 여부 체크 / 재료가 다 모였는지
    public bool IsReadyToCook()
    {
        if (CurrentRecipe == null)
            return false;

        // 모자란 재료가 하나라도 있으면 false 반환
        foreach (var req in CurrentRecipe.Data.ingredients)
        {
            // 현재 넣은 갯수 < 필요한 갯수
            if (InsertedIngredients[req.ingredient.id] < req.amount)
                return false;
        }

        // 재료가 충족되면 true 반환
        return true;
    }

    public void ClearTarget()
    {
        CurrentRecipe = null;
        InsertedIngredients.Clear();
    }
}
