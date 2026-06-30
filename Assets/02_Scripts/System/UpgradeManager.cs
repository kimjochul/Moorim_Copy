using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [SerializeField] private List<UpgradeData> upgradeList;
    
    private Dictionary<UpgradeType, UpgradeData> upgradeDataDict = new();
    private Dictionary<UpgradeType, int> levels = new();
    
    public Action SelectedOptionUpgraded; 
    
    protected override void Awake()
    {
        base.Awake();
        
        foreach (var data in upgradeList)
        {
            upgradeDataDict[data.type] = data;
        }
    }
    
    public void Init()
    {
        levels.Clear();
    }
    
    public int GetLevel(UpgradeType type)
    {
        return levels.GetValueOrDefault(type);
    }

    public void Upgrade(UpgradeType type)
    {
        UpgradeData data = GetData(type);

        int currentLevel = GetLevel(type);

        if (currentLevel >= data.MaxLevel)
            return;

        levels[type] = currentLevel + 1;
        SelectedOptionUpgraded?.Invoke();
    }
    
    public float GetValue(UpgradeType type)
    {
        int level = GetLevel(type);

        if (level <= 0)
            return 0;

        return GetData(type).levels[level - 1].value;
    }
    
    public float GetPercentValue(UpgradeType type)
    {
        return GetValue(type) / 100f;
    }
    
    public UpgradeData GetData(UpgradeType type)
    {
        if (upgradeDataDict.TryGetValue(type, out UpgradeData data))
            return data;

        Debug.LogError($"UpgradeData 없음 : {type}");
        return null;
    }

    public List<UpgradeData> GetUpgradeList()
    {
        return upgradeList;
    }
    
    public UpgradeSaveData GetSaveData()
    {
        UpgradeSaveData saveData = new();

        foreach (var pair in levels)
        {
            saveData.levels.Add(new UpgradeLevelSaveData
            {
                type = pair.Key,
                level = pair.Value
            });
        }

        return saveData;
    }
    
    public void LoadFromData(UpgradeSaveData data)
    {
        levels.Clear();

        if (data == null)
            return;

        foreach (var levelData in data.levels)
        {
            levels[levelData.type] = levelData.level;
        }
    }
    
    public bool IsUnlocked(UpgradeType type)
    {
        return GetValue(type) > 0;
    }
    
    public bool HasCharacterSkill(CharacterSkillLevel skillLevel)
    {
        return GetLevel(UpgradeType.CharacterSkill) >= (int)skillLevel;
    }
    
    public void ResetData()
    {
        levels.Clear();
    }
}
