using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
    public DailyResultSaveData dailyResult;
    public List<InventorySaveData> inventories;
    public BondSaveData bond;
    public UpgradeSaveData upgrade;
}

public enum SaveSlotType
{
    Auto,
    Manual
}
[Serializable]
public class SaveMetaData
{
    public int day;
    public int hour;
    public int minute;
    public string saveTime; // 실제 시간
}

public class SaveManager : Singleton<SaveManager>
{
    
    public bool isLoadedGame = false;
    private bool isQuitting = false;
    
    private int loadSlotIndex;
    private SaveSlotType loadType;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    // ------------------------
    // 새게임 시작
    // ------------------------
    public void StartNewGame()
    {
        Time.timeScale = 1f;
        isLoadedGame = false;
        SceneLoader.Instance.LoadScene("01_Scenes/03.Game 1", () =>
        {
            InitAll();
            InitializeNewGameData();

            UIManager.Instance.CloseAllUI();
            DayManager.Instance.RemoteDayStart(isLoadedGame);
        });
    }
    
    // ------------------------
    // 저장
    // ------------------------
    public void SaveGame(int slotIndex, SaveSlotType type)
    {
        if (isQuitting)
        {
            Debug.Log("종료 중 저장 스킵");
            return;
        }
        
        string path = GetSavePath(slotIndex, type);

        SaveData data = CreateSaveData();
        string json = JsonUtility.ToJson(data, true);
        
        SaveMetaData meta = new SaveMetaData
        {
            day = data.day.day,
            hour = data.day.hour,
            minute = data.day.minute,
            saveTime = DateTime.Now.ToString()
        };
        File.WriteAllText(path, json);
        
        Debug.Log($"저장 완료: {path}");
    }
    
    private string GetSavePath(int slotIndex, SaveSlotType type)
    {
        string fileName = type == SaveSlotType.Auto
            ? "auto_save.json"
            : $"save_{slotIndex}.json";

        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // ------------------------
    // 불러오기
    // ------------------------
    public void LoadGame(int slotIndex, SaveSlotType type)
    {
        Time.timeScale = 1f;
        isLoadedGame = true;
        
        loadSlotIndex = slotIndex;
        loadType = type;
        
        SceneLoader.Instance.LoadScene("01_Scenes/03.Game 1", () =>
        {
            InitAll();
            
            string path = GetSavePath(loadSlotIndex, loadType);

            if (!File.Exists(path))
            {
                Debug.Log("세이브 없음 → 새 게임");
                InitializeNewGameData();
                return;
            }
            else
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                ApplySaveData(data);
                
                GameManager.Instance.ResetFlow();
                UIManager.Instance.CloseAllUI();
           
                if (data.day.isDayEnded)
                {
                    GameManager.Instance.SetState(GameFlowState.Gameplay);
                    GameManager.Instance.CompleteState(GameFlowState.Gameplay);
                }
                else
                {
                    DayManager.Instance.RemoteDayStart(true);
                }
            }
        });
    }
    
    private IEnumerator StartRoutine()
    {
        yield return null; // 씬 초기화 대기
        
        InitAll();
        
        yield return null;
        yield return new WaitUntil(() => InventoryManager.Instance.IsInitialized);
        yield return null;
        
        if (isLoadedGame)
        {
            string path = GetSavePath(loadSlotIndex, loadType);
            
            if (!File.Exists(path))
            {
                Debug.Log("세이브 파일 없음 → 새 게임 시작");
            }
            else
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                ApplySaveData(data);
            }
        }
        else if (!isLoadedGame)
        {
            InitializeNewGameData(); 
        }
        
        UIManager.Instance.CloseAllUI();
        
        DayManager.Instance.RemoteDayStart(isLoadedGame);
        Debug.Log(isLoadedGame ? "로드 완료" : "새 게임 시작");
    }
    
    private void InitAll()
    {
        EconomyManager.Instance.Init();
        RecipeUnlockManager.Instance.Init();
        IngredientUnlockManager.Instance.Init();
        InventoryManager.Instance.Init();
        StoryManager.Instance.Init();
        QuestManager.Instance.Init();
        BondManager.Instance.Init();
        //UpgradeManager.Instance.Init();
    }
    
    // ------------------------
    // 데이터 생성
    // ------------------------
    private SaveData CreateSaveData()
    {
        SaveData data = new SaveData();

        if (DayManager.Instance)
            data.day = DayManager.Instance.GetSaveData();

        if (InventoryManager.Instance)
            data.inventories = InventoryManager.Instance.GetAll()
                .Where(inv => inv.Type != Constants.InventoryType.BondItem)
                .Select(inv => inv.GetSaveData()).ToList();

        if (RecipeUnlockManager.Instance)
            data.recipe = RecipeUnlockManager.Instance.GetSaveData();

        if (IngredientUnlockManager.Instance)
            data.ingredient = IngredientUnlockManager.Instance.GetSaveData();

        if (StoryManager.Instance)
            data.story = StoryManager.Instance.GetSaveData();

        if (EconomyManager.Instance)
        {
            data.economy = EconomyManager.Instance.GetSaveData();
            data.dailyResult = EconomyManager.Instance.LastDailyResult;;
        }

        if (QuestManager.Instance)
            data.quest = QuestManager.Instance.GetSaveData();
        
        if (BondManager.Instance)
            data.bond = BondManager.Instance.GetSaveData();

        if (UpgradeManager.Instance)
            data.upgrade = UpgradeManager.Instance.GetSaveData();
        
        return data;
    }

    // ------------------------
    // 데이터 적용
    // ------------------------
    private void ApplySaveData(SaveData data)
    {
        DayManager.Instance.LoadFromData(data.day);
        RecipeUnlockManager.Instance.LoadFromData(data.recipe);
        IngredientUnlockManager.Instance.LoadFromData(data.ingredient);
        if (data.inventories != null)
        {
            foreach (var invData in data.inventories)
            {
                if (invData.type == Constants.InventoryType.BondItem)
                    continue;
                
                Debug.Log($"로드 시도 타입: {invData.type}");
                var inv = InventoryManager.Instance.GetInventory(invData.type);
                Debug.Log($"찾은 인벤토리: {inv}");
                inv.LoadFromData(invData);
                
                foreach (var slot in inv.Slots)
                {
                    if (!slot.IsEmpty)
                    {
                        QuestManager.Instance.NotifyListener(new QuestEvent
                        {
                            type = QuestEventType.ItemChanged,
                            ID = slot.Item.ID,
                            currentCount = inv.GetItemCount(slot.Item.ID)
                        });
                    }
                }
            }
        }
        StoryManager.Instance.LoadFromData(data.story);
        EconomyManager.Instance.LoadFromData(data.economy);
        if (data.dailyResult != null)
        {
            EconomyManager.Instance.LoadDailyResult(data.dailyResult);
        }
        QuestManager.Instance.LoadFromData(data.quest);
        if (data.bond != null)
        {
            BondManager.Instance.LoadFromData(data.bond);
        }
        
        if (data.upgrade != null)
        {
            UpgradeManager.Instance.LoadFromData(data.upgrade);
        }
    }

    private void InitializeNewGameData()
    {
        DayManager.Instance.ResetData();
        RecipeUnlockManager.Instance.ResetData();
        IngredientUnlockManager.Instance.ResetData();
        InventoryManager.Instance.ResetData();
        StoryManager.Instance.ResetData();
        EconomyManager.Instance.ResetData();
        QuestManager.Instance.ResetData();
        BondManager.Instance.ResetData();
        //UpgradeManager.Instance.ResetData();
    }
    
    public void SaveAndQuit()
    {
        SaveGame(0, SaveSlotType.Auto);
        Application.Quit();
    }
}

[Serializable]
public class DaySaveData
{
    public int day;
    public int dayOfWeek;
    public int hour;
    public int minute;
    public bool isDayEnded;
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
    public List<int> playedStoryHistory = new();
    public List<string> flags = new();
}

[Serializable]
public class EconomySaveData
{
    public int money;
    public int fame;
    public int debt;
}

[Serializable]
public class DailyResultSaveData
{
    public int todayMoneyIncome;
    public int todayFameIncome;
    public int todayCustomerCount;

    public int happyGuests;
    public int angryGuests;
}

[Serializable]
public class InventorySaveData
{
    public Constants.InventoryType type;
    public List<SlotSaveData> slots = new();
}

[Serializable]
public class SlotSaveData
{
    public int itemID;
    public int count;

    public float spoilTimer;
    public int fridgeSpoilTimer;
}

[Serializable]
public class BondSaveData
{
    public List<int> ownedBondItemIds = new();
}

[Serializable]
public class UpgradeSaveData
{
    public List<UpgradeLevelSaveData> levels = new();
}

[Serializable]
public class UpgradeLevelSaveData
{
    public UpgradeType type;
    public int level;
}