using System.Collections;
using System.Net;
using UnityEngine;
using static Constants;

public class ServingSlot : MonoBehaviour, IInteractable
{
    [Header("��ȣ�ۿ� ĳ����")]
    [SerializeField] 
    private Transform servingPoint;

    //�丮 ������Ʈ
    private GameObject objFoodPrefab;
    //�ϼ��� �丮 ��ũ��Ʈ �־��� ����
    private GameObject currentFood;
    public float Timer { get; private set; } = 0f;
    public bool IsEmpty => this.currentFood == null;

    [SerializeField] private Inventory inventory;
    
    private Coroutine coroutineSpoil;

    public InteractMode Mode => InteractMode.AutoOnEnter;

    // 시그니처 변경 (내용은 기존 로직 유지하시면서 player 활용)
    public void Interact(PlayerController player)
    {
        // 상호작용 로직
        if (player.PlayerType == PlayerType.Sowol)
        {
            Inventory chefInventory = InventoryManager.Instance.GetInventory(InventoryType.Chef);
            InventoryManager.Instance.OpenInventory(inventory);
            InventoryManager.Instance.OpenInventory(chefInventory);
        }
        else if (player.PlayerType == PlayerType.Myungwol)
        {
            Inventory serverInventory = InventoryManager.Instance.GetInventory(InventoryType.Server);
            InventoryManager.Instance.OpenInventory(inventory);
            InventoryManager.Instance.OpenInventory(serverInventory);
        }
    }

    // 추가
    public void OnInteractEnd(PlayerController player)
    {
        // 종료 시 정리 로직
        if (player.PlayerType == PlayerType.Sowol)
        {
            Inventory chefInventory = InventoryManager.Instance.GetInventory(InventoryType.Chef);
            InventoryManager.Instance.CloseInventory(inventory);
            InventoryManager.Instance.CloseInventory(chefInventory);
        }
        else if (player.PlayerType == PlayerType.Myungwol)
        {
            Inventory serverInventory = InventoryManager.Instance.GetInventory(InventoryType.Server);
            InventoryManager.Instance.CloseInventory(inventory);
            InventoryManager.Instance.CloseInventory(serverInventory);
        }
    }
    
    /// <summary>
    /// 소월이가 배식대에 음식을 놓는 상황
    /// </summary>
    /// <param name="foodData"></param>
    public void PlaceFood(IItem foodItem)
    {
        inventory.GetItem(foodItem, 1);
        
        Timer = 0f;

        if (coroutineSpoil != null) 
            StopCoroutine(coroutineSpoil);

        coroutineSpoil = StartCoroutine(SpoilTimerRoutine());
    }
    
    /// <summary>
    /// 명월이가 배식대에서 음식을 꺼내는 상황
    /// </summary>
    /// <returns></returns>
    public IItem TakeFood(int slotIndex)
    {
        var slot = inventory.Slots[slotIndex];
        if (slot.Item == null)
        {
            Debug.Log("음식 없음");
            return null;
        }

        IItem item = slot.Item;

        inventory.RemoveItem(item.ID, 1);

        if (objFoodPrefab != null)
        {
            Destroy(this.objFoodPrefab);
            objFoodPrefab = null;
        }

        if (coroutineSpoil != null) StopCoroutine(coroutineSpoil);
        coroutineSpoil = null;
        currentFood = null;
        Timer = 0f;
        
        return item;
    }

    private IEnumerator SpoilTimerRoutine()
    {
        while (Timer < SPOIL_TIME)
        {
            Timer += Time.deltaTime;
            yield return null; 
        }

        SpoilFood();
    }
    private void SpoilFood()
    {
        Debug.Log($"{this.currentFood.name} �丮 ���");

        currentFood = null;
        Timer = 0f;
        coroutineSpoil = null;
    }
    public Vector3 GetInteractPosition()
    {
        return servingPoint.position;
    }
    
    public bool CanInteract(PlayerType playerType)
    {
        return true;
    }
}
