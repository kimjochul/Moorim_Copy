using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientAmount
{
    public Ingredient ingredient;
    public int amount;
}

[Serializable]
public class Recipe
{
    public int id;
    
    public string recipeName;

    public List<IngredientAmount> ingredients;

    public Constants.CookingMethod cookingMethod;

    public int price;

    public bool isCookCompleted;
    
    public bool isLocked;
}

// ordering, serving 기능을 위해서 GameObject로 생성하는 아이템을 만들기는 해야할 듯