using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
public class IngredientData : ItemData
{
    public bool isLocked;
    public Constants.IngredientType type; // 재료 종류
    public Constants.IngredientRarity rarity; // 재료 희귀도
}
