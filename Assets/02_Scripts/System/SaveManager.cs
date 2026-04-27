using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{   
    public int version = 1;

    public DaySaveData day;
    public QuestSaveData quest;
    public RecipeSaveData recipe;
    public IngredientSaveData ingredient;
    public StorySaveData story;
    public EconomySaveData economy;
}

public class SaveManager : Singleton<SaveManager>
{
    private string savePath;
    
    public bool isLoadedGame = false;
    
    protected override void Awake()
    {
        base.Awake();
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }
    
    // ------------------------
    // 새게임 시작
    // ------------------------
    public void StartNewGame()
    {
        isLoadedGame = false;
        StartCoroutine(StartRoutine());
    }
    
    // ------------------------
    // 저장
    // ------------------------
    public void SaveGame()
    {
        SaveData data = CreateSaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"저장 완료: {savePath}");
    }

    // ------------------------
    // 불러오기
    // ------------------------
    public void LoadGame()
    {
        isLoadedGame = true;
        StartCoroutine(StartRoutine());
    }
    
    private IEnumerator StartRoutine()
    {
        yield return null; // 씬 초기화 대기

        if (isLoadedGame)
        {
            if (!File.Exists(savePath))
            {
                Debug.Log("세이브 파일 없음 → 새 게임 시작");
            }
            else
            {
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                ApplySaveData(data);
            }
        }

        // 👉 핵심: 여기서만 DayStart
        DayManager.Instance.RemoteDayStart();
        Debug.Log(isLoadedGame ? "로드 완료" : "새 게임 시작");
    }
    
    // ------------------------
    // 데이터 생성
    // ------------------------
    private SaveData CreateSaveData()
    {
        SaveData data = new SaveData();
        
        data.day = DayManager.Instance.GetSaveData();
        data.quest = QuestManager.Instance.GetSaveData();
        data.recipe = RecipeUnlockManager.Instance.GetSaveData();
        data.ingredient = IngredientUnlockManager.Instance.GetSaveData();
        data.story = StoryManager.Instance.GetSaveData();
        data.economy = EconomyManager.Instance.GetSaveData();
        
        return data;
    }

    // ------------------------
    // 데이터 적용
    // ------------------------
    private void ApplySaveData(SaveData data)
    {
        DayManager.Instance.LoadFromData(data.day);
        QuestManager.Instance.LoadFromData(data.quest);
        RecipeUnlockManager.Instance.LoadFromData(data.recipe);
        IngredientUnlockManager.Instance.LoadFromData(data.ingredient);
        StoryManager.Instance.LoadFromData(data.story);
        EconomyManager.Instance.LoadFromData(data.economy);
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }
}

[Serializable]
public class DaySaveData
{
    public int day;
    public int dayOfWeek;
}

[Serializable]
public class QuestSaveData
{
    public List<QuestRuntimeData> activeQuests = new();
    public List<int> completedQuestIDs = new();
}

[Serializable]
public class QuestRuntimeData
{
    public int questID;

    public int rewardGold;
    public int rewardFame;

    public List<ConditionRuntimeData> conditions = new();
}

[Serializable]
public class ConditionRuntimeData
{
    public int conditionIndex;

    public int currentCount;
    
    public int targetID;       // itemID 등
    public int requiredCount;

    public int extraValue;     // failCount, serveCount 등
}
[Serializable]
public class RecipeSaveData
{
    public List<int> unlockedRecipeIDs = new();
    public List<RecipeProficiencyData> proficiencies = new();
}

[Serializable]
public class RecipeProficiencyData
{
    public int recipeID;
    public int proficiency;
}

[Serializable]
public class IngredientSaveData
{
    public List<int> unlockedIngredientIDs = new();
}

[Serializable]
public class StorySaveData
{
    public List<int> playedStoryIDs = new();
    public List<string> flags = new();
}

[Serializable]
public class EconomySaveData
{
    public int money;
    public int fame;
    public int debt;
}