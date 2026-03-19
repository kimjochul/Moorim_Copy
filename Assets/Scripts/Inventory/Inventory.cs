using System;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryType
{
    Fridge,   // 냉장고 (퀘스트 기준)
    Chef,      // 조리사 (임시 저장소)
    Server,
    MealTable,
    Ingredients,
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount = 10;
    private List<Slot> slots  = new List<Slot>();
    
    [SerializeField] private InventoryType type;
    public InventoryType Type => type;

    private void Start()
    {
        CreateSlots(slotCount);

        var itemFactory = GameObject.FindFirstObjectByType<Shop>().GetComponent<Shop>();
        itemFactory.Buy(0, 1);
    }

    private void CreateSlots(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Slot slot = Instantiate(slotPrefab, transform).GetComponent<Slot>();
            slot.SetOwner(this);
            slot.OnSlotDrop += HandleSlotDrop;
            slots.Add(slot);
        }
    }
    
    public void AddSlot(int amount)
    {
        CreateSlots(amount);
    }
    
    private void NotifyItemChanged(int itemID)
    {
        if (type != InventoryType.Fridge)
            return; 

        QuestManager.Instance.NotifyListener(new QuestEvent
        {
            type = QuestEventType.ItemChanged,
            value = itemID,
            currentCount = GetItemCount(itemID)
        });
    }
    
    private void HandleSlotDrop(Slot target, Slot source)
    {
        // 같은 인벤토리면 내가 처리
        if (target.Owner == this && source.Owner == this)
        {
            MoveItem(target, source);
        }
        // 다른 인벤토리면 "target 쪽"이 처리
        else if (target.Owner == this)
        {
            MoveItem(target, source);
        }
    }

    private void MoveItem(Slot target, Slot source)
    {
        if (source.IsEmpty)
            return;

        int itemID = source.Item.ID;

        if (target.IsEmpty && Input.GetKey(KeyCode.LeftControl))
        {
            int half = source.Count / 2;
            if (half > 0)
            {
                target.SetItem(source.Item, half);
                source.AddCount(-half);
                
                if (source.Count <= 0)
                    source.Clear();
            }
            NotifyItemChanged(itemID);
            return;
        }
        
        // 1. 빈 슬롯
        if (target.IsEmpty)
        {
            target.SetItem(source.Item, source.Count);
            source.Clear();
        }
        // 2. 같은 아이템 → 합치기
        else if (target.Item.ID == source.Item.ID)
        {
            int max = target.Item.InventoryCapacity;
            int canAdd = max - target.Count;

            if (canAdd > 0)
            {
                int move = Mathf.Min(canAdd, source.Count);
                target.AddCount(move);
                source.AddCount(-move);

                if (source.Count <= 0)
                    source.Clear();
            }
        }
        // 3. 스왑
        else
        {
            IItem tempItem = target.Item;
            int tempCount = target.Count;
            // dragslot은 원래 슬롯을 참조하는 것이기 때문에 원래 슬롯이 변경됨
            //“dragSlot은 A 슬롯 객체를 참조하고 있고, dragSlot을 통해 호출한 메서드는 A 슬롯 인스턴스의 상태를 직접 변경한다.”
            target.SetItem(source.Item, source.Count);
            source.SetItem(tempItem, tempCount);
        }

        NotifyItemChanged(itemID);
        if (!target.IsEmpty)
            NotifyItemChanged(target.Item.ID);
    }

    public void GetItem(IItem item, int count)
    {
        int itemID = item.ID;
        bool changed = false;
        
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Item.ItemName == item.ItemName && slot.Count < item.InventoryCapacity)
            {
                slot.AddCount(count);
                Debug.Log($"Added {slot.Item}");
                changed = true;
                break;
            }
            
            if(slot.IsEmpty)
            {
                slot.AddItem(item, count);
                changed = true;
                break;
            }
        }
        if (changed)
            NotifyItemChanged(itemID);
    }

    public void RemoveItem(int itemID, int removeCount)
    {
        bool changed = false;
        
        foreach (var slot in slots)
        {
            if (removeCount <= 0)
                break;
            
            if (slot.IsEmpty)
                continue;
            
            if (slot.Item.ID != itemID)
                continue;
            
            changed = true;
            
            int removable = Mathf.Min(slot.Count, removeCount);
            slot.AddCount(-removable);
            removeCount -= removable;

            if (slot.Count <= 0)
                slot.Clear();
        }
        
        if (changed)
            NotifyItemChanged(itemID);
    }
    
    public int GetItemCount(int itemID)
    {
        int count = 0;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
                continue;

            if (slot.Item.ID != itemID)
                continue;

            count += slot.Count;
        }

        return count;
    }
}
