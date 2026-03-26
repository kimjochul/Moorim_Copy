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

    public void PlaceFood(GameObject food)
    {
        this.currentFood = food;
        this.Timer = 0f;

        if (this.currentFood != null)
        {
            //�丮 ������Ʈ ����
        }

        if (this.coroutineSpoil != null) StopCoroutine(this.coroutineSpoil);

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
    public void Interact(Inventory inventory = null) 
    {
        PlayerManager.Instance.MoveSelectedUnitTo(GetInteractPosition(), () =>
        {
            // ��ȣ�ۿ� �۾�
        });
    }
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == this.playerType;
    }
}
