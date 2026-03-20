using System.Collections;
using System.Net;
using UnityEngine;
using static Constants;

public class ServingSlot : MonoBehaviour, IInteractable
{
    [Header("상호작용 캐릭터")]
    [SerializeField]
    private PlayerType playerType;
    [SerializeField] 
    private Transform servingPoint;

    //요리 오브젝트
    private GameObject objFoodPrefab;
    //완성된 요리 스크립트 넣어줄 예정
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
            //요리 오브젝트 생성
        }

        if (this.coroutineSpoil != null) StopCoroutine(this.coroutineSpoil);

        this.coroutineSpoil = StartCoroutine(SpoilTimerRoutine());
    }
    public GameObject TakeFood()
    {
        GameObject food = this.currentFood;

        if (this.objFoodPrefab != null)
        {
            //GameObject의 모델링만 변경하는 방법이 좋음
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
        Debug.Log($"{this.currentFood.name} 요리 폐기");

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
        PlayerManager.Instance.MoveSelectedUnitTo(GetInteractPosition(), () =>
        {
            // 상호작용 작업
        });
    }
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == this.playerType;
    }
}
