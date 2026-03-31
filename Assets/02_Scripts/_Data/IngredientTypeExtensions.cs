using UnityEngine;
using static Constants;

public static class IngredientTypeExtensions
{
    public static string ToKorean(this IngredientType type)
    {
        switch (type)
        {
            case IngredientType.Vegetable: return "채소";
            case IngredientType.Meats: return "고기";
            case IngredientType.Dairy: return "유제품";
            case IngredientType.Spices: return "조미료";
            case IngredientType.Processed: return "가공식재료";
            default: return "분류 미상";
        }
    }
}
