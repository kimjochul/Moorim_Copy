using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: 인벤토리랑 소통해서 슬롯에서 클릭 시 조리 화면에 아이템 개수 전송

/// <summary>
/// UI상에서 슬롯에 있는 아이템 시각적으로 표시하고 인벤토리에 어떤 아이템이 담겨져있는지 알려주기 위한 클래스, 인벤토리하고만 소통
/// </summary>
public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    public Inventory Owner { get; private set; }
    
    public IItem Item { get; private set; }
    public int Count {get; private set;}
    
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotButton;
    
    [SerializeField] private Image dragItem;  // 드래그해서 실제로 움직이는건 이미지만
    private static Slot dragSlot;             
    // dragSlot은 옮겨지는 슬롯을 참조한다. class, monobehavior, GameObject, Transform 등등은 참조타입
    
    public bool IsEmpty { get; private set; } = true;

    public event Action<Slot, Slot> OnSlotDrop; // 드래그 중인 아이템을 슬롯에서 놓을 때 호출 용
    
    // 디버그용
    [SerializeField] private ItemData debugItemData;
    [SerializeField] private int debugCount;
    private void Update()
    {
        if (Item != null)
        {
            debugItemData = Item.Data;
            debugCount = Count;
        }
        else
        {
            debugItemData = null;
            debugCount = 0;
        }
    }
    
    
    private void OnEnable()
    {
        slotIcon.gameObject.SetActive(!IsEmpty);
        
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(SetIngredientForCook);
    }

    public void SetOwner(Inventory owner)
    {
        Owner = owner;
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

    private void SetIngredientForCook()
    {
        if (Item == null)
            return;
        
        // 팝업창 나와서 개수 골라서 개수 받는 기능
        
        // 조리 시스템에 개수 넘겨주기
        CookingController.Instance.OnIngredientClicked(this);
        
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
    
    public void OnDrop(PointerEventData eventData) // 놓은 경우
    {
        if (dragSlot == null || dragSlot == this)
            return;
        
        OnSlotDrop?.Invoke(this, dragSlot);
        
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
    
    private void EndDrag()
    {
        dragItem.sprite = null;
        dragItem.gameObject.SetActive(false);
        dragItem.raycastTarget = true;
        dragSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
    }
}
