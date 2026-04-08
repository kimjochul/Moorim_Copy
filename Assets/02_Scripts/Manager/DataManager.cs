using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
using System;

[Serializable]
public class RecipeMastery
{
    public RecipeData data; 
    public int proficiency;

    public RecipeMastery(RecipeData originalData)
    {
        this.data = originalData;
        this.proficiency = 0;
    }
}
public class DataManager : Singleton<DataManager>
{
    // 상점
    private IngredientList ingredientList;

    // Book
    private AffinityNPCList affinityList;
    private RecipeList recipeList;

    private Dictionary<IngredientType, List<IngredientData>> dicIngredient = new Dictionary<IngredientType, List<IngredientData>>();
    private Dictionary<FoodType, List<RecipeMastery>> dicRecipe = new Dictionary<FoodType, List<RecipeMastery>>();

    protected override void Awake()
    {
        base.Awake();
        DataLoad();
    }
    public void DataLoad()
    {
        IngredientDataLoad();
        AffinityDataLoad();
        RecipeDataLoad();

        RecipeCategorize();
        IngredientCategorize();
    }
    private void AffinityDataLoad()
    {
        this.affinityList = Resources.Load<AffinityNPCList>("Data/AffinityNPCList");
    }
    private void IngredientDataLoad()
    {
        this.ingredientList = Resources.Load<IngredientList>("Data/IngredientList");
    }
    private void RecipeDataLoad()
    {
        this.recipeList = Resources.Load<RecipeList>("Data/RecipeList");
    }


    // 분류
    private void IngredientCategorize()
    {
        foreach (IngredientType type in System.Enum.GetValues(typeof(IngredientType)))
        {
            this.dicIngredient[type] = new List<IngredientData>();
        }

        foreach (IngredientData data in this.ingredientList.ingredients)
        {
            IngredientType targetType = data.type;

            if (this.dicIngredient.ContainsKey(targetType))
            {
                this.dicIngredient[targetType].Add(data);
            }
        }
    }
    private void RecipeCategorize()
    {
        foreach (FoodType type in System.Enum.GetValues(typeof(FoodType)))
        {
            this.dicRecipe[type] = new List<RecipeMastery>();
        }

        foreach (RecipeData data in this.recipeList.recipes)
        {
            FoodType targetType = data.foodType;

            if (this.dicRecipe.ContainsKey(targetType))
            {
                RecipeMastery mastery = new RecipeMastery(data);
                this.dicRecipe[targetType].Add(mastery);
            }
        }
    }

    public List<IngredientData> GetIngredientList(IngredientType type)
    {
        return this.dicIngredient[type];
    }
    public List<AffinityNPCData> GetAffinityNPCList()
    {
        return this.affinityList.listAffinityNPC;
    }
    public List<RecipeMastery> GetRecipeList(FoodType type)
    {
        return this.dicRecipe[type];
    }
    public void AddRecipeProficiency(RecipeData targetRecipe, int amount)
    {
        FoodType targetType = targetRecipe.foodType;

        if (this.dicRecipe.ContainsKey(targetType))
        {
            foreach (RecipeMastery mastery in this.dicRecipe[targetType])
            {
                if (mastery.data == targetRecipe)
                {
                    mastery.proficiency = Mathf.Clamp(mastery.proficiency + amount, 0, 100);

                    return;
                }
            }
        }
    }
}
