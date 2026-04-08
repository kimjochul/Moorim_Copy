using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static Constants;
using System;
using System.Linq;

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
    // 모든 아이템
    private ItemDatabase itemDatabase;

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
        ItemDatabaseDataLoad();
        IngredientDataLoad();
        AffinityDataLoad();
        RecipeDataLoad();

        RecipeCategorize();
        IngredientCategorize();
    }
    private void ItemDatabaseDataLoad()
    {
        this.itemDatabase = Resources.Load<ItemDatabase>("Data/ItemDatabase");
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


    // 재료 분류
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
    // 레시피 분류
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
    // ItemDatabase에서 검색(ItemData 추출)
    public ItemData GetItemData(int id)
    {
        return this.itemDatabase.Get(id);
    }
    // ItemDatabase에서 검색(이름 추출)
    public string GetItemName(int id)
    {
        return this.itemDatabase.Get(id).itemName;
    }
    // 재료 타입 리스트 반환
    public List<IngredientData> GetIngredientList(IngredientType type)
    {
        return this.dicIngredient[type];
    }
    // id로 검색해서 재료 데이터 반환
    public IngredientData GetIngredientData(int id)
    {
        IngredientData findData = this.dicIngredient.Values
            .SelectMany(list => list)
            .FirstOrDefault(data => data.id == id);

        return findData;
    }
    // id로 검색해서 재료 데이터 이름 반환
    public string GetIngredientName(int id)
    {
        return GetIngredientData(id).itemName;
    }
    // 인연 캐릭터 리스트 반환
    public List<AffinityNPCData> GetAffinityNPCList()
    {
        return this.affinityList.listAffinityNPC;
    }
    // id로 검색해서 레시피 데이터 반환
    public RecipeData GetRecipeMastery(int id)
    {
        RecipeData findData = this.dicRecipe.Values
            .SelectMany(list => list)
            .FirstOrDefault(mastery => mastery.data.id == id).data;

        return findData;
    }
    // id로 검색해서 레시피 이름 반환
    public string GetRecipeName(int id)
    {
        return GetRecipeMastery(id).recipeName;
    }
    // 해당 레시피 숙련도 상승
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
