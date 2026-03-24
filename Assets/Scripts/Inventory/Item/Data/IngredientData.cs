using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
public class IngredientData : ItemData
{
    public bool isLocked;
    public Constants.IngredientType type;
}
