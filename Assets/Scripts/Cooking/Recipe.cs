using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientAmount
{
    public IngredientData ingredient;
    public int amount;
}

[Serializable]
public class Recipe
{
    public RecipeData Data { get; private set; }
    public bool IsCookCompleted { get; private set; }

    public Recipe(RecipeData data)
    {
        Data = data;
    }

    public void CompleteCook()
    {
        IsCookCompleted = true;
    }
}

// ordering, serving 기능을 위해서 GameObject로 생성하는 아이템을 만들기는 해야할 듯