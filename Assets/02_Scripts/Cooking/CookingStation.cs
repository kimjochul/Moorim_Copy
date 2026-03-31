using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum CookingState { Idle, Cooking, Done }

public class CookingStation : MonoBehaviour, IInteractable
{
    // [Header("UI")]
    // [SerializeField] private GameObject resultObj;
    // [SerializeField] private Image resultFoodIcon;

    [Header("조리 설정")]
    [SerializeField] private float cookTime = 15f;

    public CookingState CurrentState { get; private set; } = CookingState.Idle;
    private Recipe currentRecipe;
    
    void Start()
    {
        // if (resultObj != null)
        //     resultObj.SetActive(false);
    }
    
    
    // 플레이어가 조리대에 상호작용했을 때 실행되어야할 기능
    public void Interact(Inventory playerInventory)
    {
        if (CurrentState == CookingState.Idle && CookingController.Instance != null)
        {
            CookingController.Instance.OpenCookingSystem(this);
        }
        else if (CurrentState == CookingState.Cooking)
        {
            Debug.Log("보글보글.. 현재 조리 중입니다!");
        }
        else if (CurrentState == CookingState.Done)
        {
            CollectFood(playerInventory);
        }
    }

    public bool CanInteract(Constants.PlayerType playerType)
    {
        if(playerType == Constants.PlayerType.Sowol)
            return true;
        
        return false;
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