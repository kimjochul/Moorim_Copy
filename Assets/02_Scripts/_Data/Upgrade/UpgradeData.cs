using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrade/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public int id;
    public string upgradeName;
    
    [TextArea]
    public string description;

    public UpgradeType type;

    public List<UpgradeLevelData> levels;

    public int maxLevel;
}

[Serializable]
public class UpgradeLevelData
{
    public List<UpgradeCost> costs;

    [TextArea]
    public string effectDescription;
}

[Serializable]
public class UpgradeCost
{
    public int itemID;
    public int count;
    public int money;
}

public enum UpgradeType
{
    AutoIngredient,
    TableCount,
    CharacterSkill,
    CookSpeed,
    CookAmount
}