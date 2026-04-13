using System;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(GuestBase))]
public class GuestInteractable : MonoBehaviour, IInteractable
{
    private GuestBase myBrain;
    public GuestBase MyBrain => myBrain;

    private Inventory guestInven;
 
 
    void Awake()
    {
        myBrain = GetComponent<GuestBase>();
        guestInven = InventoryManager.Instance.GetInventory(InventoryType.Guest);
    }
 
    // IInteractable 구현 
 
    // 손님은 클릭해서 상호작용 (ClickToInteract)
    public InteractMode Mode => InteractMode.ClickToInteract;
 
    // 서빙 캐릭터만 상호작용 가능
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == PlayerType.Myungwol;
    }
 
    // 명월이가 손님을 클릭했을 때 맵의 어디로 걸어와야 하는가?
    public Vector3 GetInteractPosition()
    {
        return transform.position;
    }
 
    // 명월이가 손님 근처에 도착해서 상호작용을 완료했을 때 벌어질 일
    public void Interact(PlayerController player)
    {
        if (myBrain.currentState == GuestState.WaitOrder)
            myBrain.TakeOrder();
        else if (myBrain.currentState == GuestState.WaitFood)
        {
            OnOpenInventories();
        }
        else if (myBrain.currentState == GuestState.Exit)
        {
            ThiefGuyGuest thief = myBrain as ThiefGuyGuest;
            
            if (thief != null)
                thief.CatchThief();
        }
        else
        {
            Debug.Log($"상호작용 무시 (현재 상태: {myBrain.currentState})");
        }
    }
 
    public void OnInteractEnd(PlayerController player)
    {
        // ClickToInteract — 트리거 퇴장으로 호출되지 않음
    }

    private void OnOpenInventories()
    {
        Inventory serverInven = InventoryManager.Instance.GetInventory(InventoryType.Server);
        InventoryManager.Instance.OpenInventory(serverInven);
        InventoryManager.Instance.OpenInventory(guestInven);
    }
    
    /// <summary>
    /// guest inven에 물건 넣기
    /// </summary>
    /// <param name="sourceInven"></param>
    /// <param name="sourceSlot"></param>
    public void PlaceFood(Inventory sourceInven, Slot sourceSlot)
    {
        sourceInven.MoveSlotTo(guestInven, sourceSlot, 1);
    }
    
    /// <summary>
    /// guest inven에서 물건 꺼낼 때
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="targetInven"></param>
    /// <returns></returns>
    public IItem TakeFood(int slotIndex, Inventory targetInven)
    {
        var slot = guestInven.Slots[slotIndex];
        if (slot.Item == null)
        {
            Debug.Log("음식 없음");
            return null;
        }

        IItem item = slot.Item;

        guestInven.MoveSlotTo(targetInven, slot, 1);
        guestInven.RemoveItem(item.ID, 1);
        
        return item;
    }

    /// <summary>
    /// 인스펙터 창에서 확인 버튼에 등록할 함수 
    /// </summary>
    /// <param name="deliveredFood"></param>
    /// <param name="quality"></param>
    public void CallReceiveFood()
    {
        foreach (var slot in guestInven.Slots)
        {
            if (slot.Item == null)
                continue;

            if (slot.Item.Data is FoodData deliveredFood)
            {
                FoodQuality quality = FoodQuality.Normal;
                myBrain.ReceiveFood(deliveredFood, quality);
                guestInven.RemoveItem(deliveredFood.id, slot.Count);
            }
        }
        
        foreach (var slot in guestInven.Slots)
            slot.Clear();
        
        Inventory serverInven = InventoryManager.Instance.GetInventory(InventoryType.Server);
        InventoryManager.Instance.CloseInventory(serverInven);
        InventoryManager.Instance.CloseInventory(guestInven);
    }
}
