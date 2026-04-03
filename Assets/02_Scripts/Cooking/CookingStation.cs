using System;
using System.Collections;
using static Constants;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CookingState { Idle, Cooking, Done }

public class CookingStation : MonoBehaviour, IInteractable
{
    // [Header("UI")]
    // [SerializeField] private GameObject resultObj;
    // [SerializeField] private Image resultFoodIcon;

    [Header("상호작용 위치")]
    [SerializeField] private Transform interactPoint;

    [Header("조리 설정")]
    [SerializeField] private float cookTime = 15f;

    public CookingState CurrentState { get; private set; } = CookingState.Idle;
    private Recipe currentRecipe;

    // 현재 이 조리대를 사용 중인 플레이어
    private PlayerController currentUser;
    
    void Start()
    {
        // if (resultObj != null)
        //     resultObj.SetActive(false);
    }
    
     // 조리대는 ClickToInteract — UI 닫고 재클릭 가능
    public InteractMode Mode => InteractMode.ClickToInteract;
 
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == PlayerType.Sowol;
    }

    // 플레이어가 조리대에 상호작용했을 때 실행되어야할 기능
    public void Interact(PlayerController player)
    {
        currentUser = player;
 
        if (CurrentState == CookingState.Idle && CookingController.Instance != null)
            CookingController.Instance.OpenCookingSystem(this);
        else if (CurrentState == CookingState.Cooking)
        {
            Debug.Log("보글보글.. 현재 조리 중입니다!");
 
            // 조리 중이면 상호작용 상태를 바로 해제
            // (조리는 백그라운드에서 진행되고, 소월은 자유롭게 움직일 수 있음)
            player.EndInteract();
        }
        else if (CurrentState == CookingState.Done)
        {
            // 완성된 음식 수거
            Inventory sowolInven = InventoryManager.Instance.GetInventory(InventoryType.Chef);
            CollectFood(sowolInven);
            player.EndInteract();
        }
    }

    public void OnInteractEnd(PlayerController player)
    {
        // ClickToInteract 모드이므로 트리거 퇴장으로는 호출되지 않음
        // 조리대는 자체적으로 EndInteract를 관리
    }

    public Vector3 GetInteractPosition()
    {
        return interactPoint != null ? interactPoint.position : transform.position;
    }


    // 조리 버튼이 눌렸을 때 실행되어야할 기능
    public void StartCooking(Recipe recipe)
    {
        currentRecipe = recipe;
        CurrentState = CookingState.Cooking;
        
        StartCoroutine(CookingRoutine());
    }

    private IEnumerator CookingRoutine()
    {
        // TO DO : 애니메이션 및 SFX 실행
        // TO DO : 요리 진행도에 따른 게이지바 상승
        
        yield return new WaitForSeconds(cookTime);

        // 요리 완료 시 현재 플레이어 상태를 interact -> idle로 바꿔서 다시 클릭, 캐릭터 변경이 가능하게
        PlayerManager.Instance.StopCurrentPlayer(); 
        
        CurrentState = CookingState.Done;

        // resultFoodIcon.sprite = currentRecipe.Data.icon;
        // resultObj.SetActive(true);
    }

    private void CollectFood(Inventory targetInventory)
    {
        if (targetInventory != null)
        {
            Item resultItem = new Item(currentRecipe.Data.result);
            targetInventory.GetItem(resultItem, 1);

            CurrentState = CookingState.Idle;
            currentRecipe = null;
            // resultObj.SetActive(false);
        }
    }

    Vector3 IInteractable.GetInteractPosition()
    {
        return default;
    }
}