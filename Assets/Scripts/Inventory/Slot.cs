using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
{
    public IItem Item { get; private set; }
    public int Count {get; private set;}
    [field:SerializeField]public bool CountForQuest { get; private set; }
    
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image slotIcon;

    [SerializeField] private Image dragItem;  // 드래그해서 실제로 움직이는건 이미지만
    private static Slot dragSlot;             
    // dragSlot은 옮겨지는 슬롯을 참조한다. class, monobehavior, GameObject, Transform 등등은 참조타입
        
    public bool IsEmpty { get; private set; } = true;

    private void OnEnable()
    {
        slotIcon.gameObject.SetActive(!IsEmpty);
    }

    public void AddItem(IItem item, int count)
    {
        this.Item = item;
        IsEmpty = false;
        slotIcon.sprite = item.Icon;
        Count = count;
        countText.text = Count.ToString();
        
        slotIcon.gameObject.SetActive(true);
    }

    private void UseItem()
    {
        if (Item == null)
            return;
        
        if (Item == null)
            return;

        Item.Use();
        AddCount(-1);
        
        if (Count <= 0)
        {
            Count = 0;
            countText.text = Count.ToString();
            Item = null;
            IsEmpty = true;
            slotIcon.gameObject.SetActive(false);
            slotIcon.sprite = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)  // 드래그 시작
    {
        if (IsEmpty)
            return;

        dragSlot = this;  // 참조 설정 dragslot을 통해 원래 슬롯에 접근
        dragItem.sprite = Item.Icon;
        dragItem.gameObject.SetActive(true);
        dragItem.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) // 드래그 중
    {
        dragItem.transform.position = eventData.position;
    }
    
    public void OnDrop(PointerEventData eventData) // 다른 UI창 위에서 놓은 경우
    {
        if (dragSlot == null || dragSlot == this)
            return;
        
        if (IsEmpty && Input.GetKey(KeyCode.LeftControl))
        {
            int half = dragSlot.Count / 2;
            if (half > 0)
            {
                SetItem(dragSlot.Item, half);
                dragSlot.AddCount(-half);
            }
            EndDrag();
            return;
        }

        // 1. 빈 슬롯 → 전부 이동
        if (IsEmpty)
        {
            SetItem(dragSlot.Item, dragSlot.Count);
            dragSlot.Clear();
            EndDrag();
            return;
        }

        // 2. 같은 아이템 → 합치기
        if (Item.ItemName == dragSlot.Item.ItemName)
        {
            int max = Item.InventoryCapacity;
            int canAdd = max - Count;

            if (canAdd > 0)
            {
                int move = Mathf.Min(canAdd, dragSlot.Count);
                AddCount(move);
                dragSlot.AddCount(-move);

                if (dragSlot.Count <= 0)
                    dragSlot.Clear();
            }

            EndDrag();
            return;
        }

        // 3. 다른 아이템 → 스왑
        SwapWith(dragSlot);
        EndDrag();
    }
    
    public void SetItem(IItem newItem, int newCount)
    {  
        this.Item = newItem;
        Count = newCount;
        
        if (newItem == null)
        {
            IsEmpty = true;
            Count = 0;
            countText.text = Count.ToString();
            slotIcon.sprite = null;
            slotIcon.gameObject.SetActive(false);
        }
        else
        {
            IsEmpty = false;
            slotIcon.sprite = newItem.Icon;
            countText.text = Count.ToString();
            slotIcon.gameObject.SetActive(true);
        }
    }

    public void AddCount(int add)
    {
        Count += add;
        countText.text = Count.ToString();
    }
    
    public void Clear()
    {
        SetItem(null, 0);
    }
    
    private void SwapWith(Slot other)
    {
        IItem tempItem = Item;
        int tempCount = Count;
        // dragslot은 원래 슬롯을 참조하는 것이기 때문에 원래 슬롯이 변경됨
        //“dragSlot은 A 슬롯 객체를 참조하고 있고, dragSlot을 통해 호출한 메서드는 A 슬롯 인스턴스의 상태를 직접 변경한다.”
        SetItem(other.Item, other.Count);
        other.SetItem(tempItem, tempCount);
    }
    
    private void EndDrag()
    {
        dragItem.sprite = null;
        dragItem.gameObject.SetActive(false);
        dragItem.raycastTarget = true;
        dragSlot = null;
    }
}
