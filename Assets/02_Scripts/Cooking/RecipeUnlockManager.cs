using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUnlockManager : Singleton<RecipeUnlockManager>
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
        if (unlockedRecipes.Contains(recipeID))
            return;
        
        Debug.Log("Unlocking recipe " + recipeID);
        unlockedRecipes.Add(recipeID);
    }

    public RecipeData RandomUnlockedRecipe()
    {
        var list = new List<int>(unlockedRecipes);

        if (list.Count == 0)
            return null;
        
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        int recipeID = list[randomIndex];

        return DataManager.Instance.GetRecipeMasteryData(recipeID).data;
    }
}
