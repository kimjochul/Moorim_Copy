using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientList", menuName = "Scriptable Objects/IngredientList")]
public class IngredientList : ScriptableObject
{
    public List<IngredientData> ingredients;
}
