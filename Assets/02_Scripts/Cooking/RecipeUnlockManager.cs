using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUnlockManager : MonoBehaviour
{
    [SerializeField] private RecipeList recipeList;
    private HashSet<int> unlockedRecipes = new HashSet<int>();

    private void Start()
    {
        foreach (var recipe in recipeList.recipes)
        {
            if (!unlockedRecipes.Contains(recipe.id) && recipe.isLocked == false)
                Unlock(recipe.id);
        }
    }

    public bool IsUnlocked(int recipeID)
    {
        return unlockedRecipes.Contains(recipeID);
    }

    public void Unlock(int recipeID)
    {
        Debug.Log("Unlocking recipe " + recipeID);
        unlockedRecipes.Add(recipeID);
    }
}
