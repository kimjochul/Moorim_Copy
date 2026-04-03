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

    private Coroutine coroutineSpoil;

    // 추가
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
    
    public void PlaceFood(GameObject food)
    {
        this.currentFood = food;
        this.Timer = 0f;

        if (this.currentFood != null)
        {
            //�丮 ������Ʈ ����
        }

        if (this.coroutineSpoil != null) 
            StopCoroutine(this.coroutineSpoil);

        this.coroutineSpoil = StartCoroutine(SpoilTimerRoutine());
    }
    public GameObject TakeFood()
    {
        GameObject food = this.currentFood;

        if (this.objFoodPrefab != null)
        {
            //GameObject�� �𵨸��� �����ϴ� ����� ����
            Destroy(this.objFoodPrefab);
            this.objFoodPrefab = null;
        }

        if (this.coroutineSpoil != null) StopCoroutine(this.coroutineSpoil);
        this.coroutineSpoil = null;

        this.currentFood = null;
        this.Timer = 0f;

        return food;
    }

    private IEnumerator SpoilTimerRoutine()
    {
        while (this.Timer < SPOIL_TIME)
        {
            this.Timer += Time.deltaTime;
            yield return null; 
        }

        SpoilFood();
    }
    private void SpoilFood()
    {
        Debug.Log($"{this.currentFood.name} �丮 ���");

        this.currentFood = null;
        this.Timer = 0f;
        this.coroutineSpoil = null;
    }
    public Vector3 GetInteractPosition()
    {
        return this.servingPoint.position;
    }
    public void Interact() 
    {
        // 상호 작용
    }
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == this.playerType;
    }
}
