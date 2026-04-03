using System.Collections;
using System.Net;
using UnityEngine;
using static Constants;

public class ServingSlot : MonoBehaviour, IInteractable
{
    [Header("��ȣ�ۿ� ĳ����")]
    [SerializeField]
    private PlayerType playerType;
    [SerializeField] 
    private Transform servingPoint;

    //�丮 ������Ʈ
    private GameObject objFoodPrefab;
    //�ϼ��� �丮 ��ũ��Ʈ �־��� ����
    private GameObject currentFood;
    public float Timer { get; private set; } = 0f;
    public bool IsEmpty => this.currentFood == null;

    private Inventory inventory;
    
    private FoodData currentItem;
    private FoodData previousItem;
    
    private Coroutine coroutineSpoil;

    public InteractMode Mode => InteractMode.AutoOnEnter;

    // 시그니처 변경 (내용은 기존 로직 유지하시면서 player 활용)
    public void Interact(PlayerController player)
    {
        // 상호작용 로직
    }

    // 추가
    public void OnInteractEnd(PlayerController player)
    {
        // 종료 시 정리 로직
    }
    
    /// <summary>
    /// 소월이가 배식대에 음식을 놓는 상황
    /// </summary>
    /// <param name="foodData"></param>
    public FoodData PlaceFood(FoodData foodData)
    {
        currentItem = foodData;
        currentFood = foodData.foodPrefab;
        Timer = 0f;

        if (currentFood != null) // 기존 배식대에 음식이 있으면 인벤토리에 되돌려주기 아니면 기능상 아예 버리는 선택지가 있을 수도
        {
            return previousItem;
        }

        if (coroutineSpoil != null) 
            StopCoroutine(this.coroutineSpoil);

        this.coroutineSpoil = StartCoroutine(SpoilTimerRoutine());
        return null;
    }
    
    /// <summary>
    /// 명월이가 배식대에서 음식을 꺼내는 상황
    /// </summary>
    /// <returns></returns>
    public GameObject TakeFood()
    {
        GameObject food = this.currentFood;

        if (this.objFoodPrefab != null)
        {
            //GameObject�� �𵨸��� �����ϴ� ����� ����
            Destroy(this.objFoodPrefab);
            objFoodPrefab = null;
        }

        if (coroutineSpoil != null) StopCoroutine(coroutineSpoil);
        coroutineSpoil = null;

        currentFood = null;
        Timer = 0f;

        return food;
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
        return playerType == this.playerType;
    }
}
