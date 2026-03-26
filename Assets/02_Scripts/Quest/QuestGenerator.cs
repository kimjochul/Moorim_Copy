using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    [SerializeField] private QuestPool questPool;
    [SerializeField] private Inventory inventory;
    
    [SerializeField] private RecipeUnlockManager recipeUnlockManager;
    [SerializeField] private RecipeList allRecipes;
    
    /// <summary>
    /// 해금이 되어있는 아이템을 필요로 하는 퀘스트만 나오게 설정해야함
    /// 퀘스트 풀에 있는 데이터들 중 하나를 랜덤으로 골라서, 리턴
    /// </summary>
    public Quest GenerateRandomQuest()
    {
        if (questPool.quests.Count == 0)
            return null;
        
        List<QuestData> validTemplates = new List<QuestData>();

        foreach (var template in questPool.quests)
        {
            if (IsValidTemplate(template))
            {
                validTemplates.Add(template);
                Debug.Log(template.name);   
            }
        }

        if (validTemplates.Count == 0)
        {
            Debug.LogWarning("유효한 퀘스트 없음");
            return null;
        }

        Debug.Log(validTemplates.Count);
        QuestData selected = validTemplates[Random.Range(0, validTemplates.Count)];
        
        Quest quest = new Quest(selected, inventory);
        RandomizeQuest(quest);
        
        Debug.Log("정해진 퀘스트" + selected.name);
        
        return quest;
    }
    
    /// <summary>
    /// 데이터로 들어온 퀘스트가 필요로 하는 아이템이 언락됐는지 확인하고 맞을 경우 템플릿에 넣을 수 있게 true를 리턴하는 함수 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool IsValidTemplate(QuestData data)
    {
        foreach (var condition in data.conditions)
        {
            if (condition is ItemCondition itemCondition)
            {
                if (itemCondition.useFoodType)
                {
                    if (!HasRecipeByType(itemCondition.foodType))
                        return false;
                }
                else if (itemCondition.useRandomItem)
                {
                    if (!HasAnyUnlockedRecipe())
                        return false;
                }
                else
                {
                    if (!recipeUnlockManager.IsUnlocked(itemCondition.itemID))
                        return false;
                }
            }
        }
        return true;
    }
    
    /// <summary>
    /// 타입으로 검색했을 때 만약 언락된 아이템이 하나도 없을 경우 unvalid라고 표시
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool HasRecipeByType(Constants.FoodType type)
    {
        foreach (var recipe in allRecipes.recipes)
        {
            if (!recipeUnlockManager.IsUnlocked(recipe.id))
                continue;

            if (recipe.foodType == type)
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 만약 무작위 음식 중에 언락된게 아예 없을 경우
    /// </summary>
    /// <returns></returns>
    private bool HasAnyUnlockedRecipe()
    {
        foreach (var recipe in allRecipes.recipes)
        {
            if (recipeUnlockManager.IsUnlocked(recipe.id))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 타입을 기준으로 현재 언락된 레시피들 중에서(퀘스트 완료 가능한 것) 랜덤 음식을 받아오는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private RecipeData GetRandomRecipeByType(Constants.FoodType type)
    {
        List<RecipeData> list = new List<RecipeData>();

        foreach (var recipe in allRecipes.recipes)
        {
            if (!recipeUnlockManager.IsUnlocked(recipe.id))
                continue;

            if (recipe.foodType == type)
                list.Add(recipe);
        }

        if (list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }
    
    /// <summary>
    /// 무작위 레시피 리턴 함수
    /// </summary>
    /// <returns></returns>
    private RecipeData GetRandomUnlockedRecipe()
    {
        List<RecipeData> list = new List<RecipeData>();

        foreach (var recipe in allRecipes.recipes)
        {
            //Debug.Log($"레시피 체크: {recipe.recipeName}, unlocked: {recipeUnlockManager.IsUnlocked(recipe.id)}");
            
            if (recipeUnlockManager.IsUnlocked(recipe.id))
                list.Add(recipe);
        }
        
        Debug.Log($"언락된 레시피 개수: {list.Count}");

        if (list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }
    
    /// <summary>
    /// 아이디 기반 레시피 탐색 함수
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private RecipeData GetRecipeByID(int id)
    {
        foreach (var recipe in allRecipes.recipes)
        {
            if (recipe.id == id)
                return recipe;
        }

        return null;
    }
    
    /// <summary>
    /// 퀘스트에 있는 퀘스트 조건의 퀘스트 조건들의 완료 개수 랜덤 설정
    /// </summary>
    /// <param name="quest"></param>
    public void RandomizeQuest(Quest quest)
    {
        foreach (var condition in quest.Conditions)
        {
            if (condition is ItemCondition itemCondition)
            {
                Debug.Log($"useRandomItem: {itemCondition.useRandomItem}, useFoodType: {itemCondition.useFoodType}, itemID: {itemCondition.itemID}");
                RecipeData recipe = null;
                if (itemCondition.useFoodType)
                {
                    recipe = GetRandomRecipeByType(itemCondition.foodType);

                    if (recipe != null)
                        itemCondition.itemID = recipe.id;
                }
                else if (itemCondition.useRandomItem)
                {
                    recipe = GetRandomUnlockedRecipe();

                    if (recipe != null)
                        itemCondition.itemID = recipe.id;
                }
                else
                    recipe = GetRecipeByID(itemCondition.itemID);
                
                itemCondition.requiredCount = Random.Range(1, 1);
                
                if (recipe != null)
                    Debug.Log($"{recipe.recipeName} / require count: {itemCondition.requiredCount}");
                else
                    Debug.LogWarning("레시피 없음");
            }

            if (condition is FailLimitCondition failCondition)
            {
                failCondition.maxFailCount = Random.Range(1, 5);
            }
        }
    }
}
