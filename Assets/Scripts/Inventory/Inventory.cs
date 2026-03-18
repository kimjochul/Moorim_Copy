using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    
    public void GetItem(IItem item)
    {
        int itemID = item.ID;
        QuestManager.Instance.NotifyListener(
            new QuestEvent
            {
                type = QuestEventType.ItemGet,
                value = item.ID
            });
        
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

    public void RemoveItem(int itemID, int removeCount)
    {
        foreach (var slot in slots)
        {
            if (removeCount <= 0)
                return;
            
            if (slot.IsEmpty)
                continue;
            
            if (slot.Item.ID != itemID)
                continue;
            
            int removable = Mathf.Min(slot.Count, removeCount);
            slot.AddCount(-removable);
            removeCount -= removable;

            if (slot.Count <= 0)
                slot.Clear();
        }
    }
    
    public int GetItemCount(int itemID)
    {
        int count = 0;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
                continue;

            if (!slot.CountForQuest)
                continue;

            if (slot.Item.ID != itemID)
                continue;

            count += slot.Count;
        }

        return count;
    }
}
