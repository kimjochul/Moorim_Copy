using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class DataManager : Singleton<DataManager>
{
    private IngredientList ingredientList;
    private RecipeList recipeList;
    private QuestList questList;

    private Dictionary<IngredientType, List<IngredientData>> dicIngredient = new Dictionary<IngredientType, List<IngredientData>>();

    void Start()
    {
        DataLoad();
    }
    public void DataLoad()
    {
        IngredientDataLoad();
        RecipeDataLoad();
        QuestDataLoad();

        IngredientCategorize();
    }
    private void IngredientDataLoad()
    {
        this.ingredientList = Resources.Load<IngredientList>("Data/IngredientList");
    }
    private void RecipeDataLoad()
    {
        this.recipeList = Resources.Load<RecipeList>("Data/RecipeList");
    }
    private void QuestDataLoad()
    {
        this.questList = Resources.Load<QuestList>("Data/QuestList");
    }

    // ºÐ·ù
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
    public List<IngredientData> GetIngredientList(IngredientType type)
    {
        return this.dicIngredient[type];
    }
}
