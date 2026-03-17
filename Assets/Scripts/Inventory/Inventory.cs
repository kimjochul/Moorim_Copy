using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    
    public void GetItem(IItem item)
    {
        string questName = item.ItemName.Replace(" ", ""); // 이름 정하는 기준 + 설정 필요
        QuestManager.Instance.NotifyListener(questName);
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Item.ItemName == item.ItemName)
            {
                slot.AddCount(1);
                Debug.Log($"Added {slot.Item}");
                return;
            }
            
            if(slot.IsEmpty)
            {
                slot.AddItem(item, 1);
                return;
            }
        }
    }

    public void RemoveItem(string itemName, int removeCount)
    {
        foreach (var slot in slots)
        {
            if (removeCount <= 0)
                return;
            
            if (slot.IsEmpty)
                continue;
            
            if (slot.Item.ItemName != itemName)
                continue;
            
            int removable = Mathf.Min(slot.Count, removeCount);
            slot.AddCount(-removable);
            removeCount -= removable;

            if (slot.Count <= 0)
                slot.Clear();
        }
    }
    
    public int GetItemCount(string itemName)
    {
        int count = 0;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
                continue;

            if (!slot.CountForQuest)
                continue;

            if (slot.Item.ItemName != itemName)
                continue;

            count += slot.Count;
        }

        return count;
    }
}
