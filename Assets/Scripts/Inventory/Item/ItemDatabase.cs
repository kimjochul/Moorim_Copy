using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
    private Dictionary<int, ItemData> itemMap;

    public void Init()
    {
        itemMap = new Dictionary<int, ItemData>();

        foreach (var item in items)
        {
            itemMap[item.id] = item;
        }
    }
    
    public ItemData Get(int id)
    {
        if (itemMap == null)
            Init();

        if (itemMap != null) return itemMap[id];
        
        return null;
    }
}
