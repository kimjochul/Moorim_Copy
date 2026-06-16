using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private Dictionary<UpgradeType, int> levels;

    public int GetLevel(UpgradeType type)
    {
        return levels[type];
    }

    public void Upgrade(UpgradeType type)
    {
        levels[type]++;
    }
}
