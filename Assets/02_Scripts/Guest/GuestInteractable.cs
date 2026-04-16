using System;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(GuestBase))]
public class GuestInteractable : MonoBehaviour, IInteractable
{
    private GuestBase myBrain;
    public GuestBase MyBrain => myBrain;
 
    void Awake()
    {
        myBrain = GetComponent<GuestBase>();
    }
 
    // IInteractable 구현 
 
    // 손님은 클릭해서 상호작용 (ClickToInteract)
    public InteractMode Mode => InteractMode.AutoOnEnter;
 
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
            // Inventory serverInven = InventoryManager.Instance.GetInventory(InventoryType.Server);
            // InventoryManager.Instance.OpenInventory(serverInven);
        }
        else if (myBrain.currentState == GuestState.Exit)
        {
            ThiefGuyGuest thief = myBrain as ThiefGuyGuest;
            
            if (thief != null)
                thief.CatchThief();
        }
    }
 
    public void OnInteractEnd(PlayerController player)
    {
        if (player.PlayerType == PlayerType.Myungwol)
        {
            Inventory serverInven = InventoryManager.Instance.GetInventory(InventoryType.Server);
            InventoryManager.Instance.CloseInventory(serverInven);
        }
    }
    
    public void DeliverFood(Inventory serverInven, Slot slot)
    {
        FoodData food = slot.Item.Data as FoodData;
        if (food == null) return;

        serverInven.RemoveItem(slot.Item.ID, 1);
        myBrain.ReceiveFood(food, FoodQuality.Normal);

        InventoryManager.Instance.CloseInventory(serverInven);
    }
}
