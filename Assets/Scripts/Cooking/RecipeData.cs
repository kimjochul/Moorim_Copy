using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/RecipeData")]
public class RecipeData : ScriptableObject
{
    public int id;
    public string recipeName;
    public List<IngredientAmount> ingredients;
    public Constants.CookingMethod cookingMethod;
    public Sprite icon;
    public int price;
    public bool isLocked;
    public string description;
}
