using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    [SerializeField] private float cookTime = 15f;

    private bool isCooking = false;

    // 테스트용 메소드
    private void OnMouseDown()
    {
        Debug.Log("솥단지 마우스 클릭됨!");
        Interact(); 
    }
    
    // 플레이어가 조리대에 상호작용했을 때 실행되어야할 기능
    private void Interact()
    {
        if (isCooking)
        {
            Debug.Log("보글보글.. 현재 조리 중입니다!");
            return;
        }

        if (CookingController.Instance != null)
            CookingController.Instance.OpenCookingSystem(this);
        else
            Debug.Log("CookingController Instance를 찾을 수 없음");
            
    }

    // 조리 버튼이 눌렸을 때 실행되어야할 기능
    public void StartCooking(Recipe recipe)
    {
        isCooking = true;
        Debug.Log($"{recipe.Data.recipeName} 요리 시작!");
        
        StartCoroutine(CookingRoutine(recipe));
    }

    private IEnumerator CookingRoutine(Recipe recipe)
    {
        // TO DO : 애니메이션 및 SFX 실행
        
        yield return new WaitForSeconds(cookTime);

        isCooking = false;
        Debug.Log("요리 완성!");

        // TO DO : 완성품 오브젝트를 맵 위에 스폰
    }
    //// 요리가 되는 동안 해야하는 기능 ////
    // 15초 타이머
    // UI 비활성화
    // 냄비 애니메이션, SFX 실행
    // 15초 끝날경우
    // 플레이어 데이터(요리 횟수)를 읽어서 음식 퀄리티(보통/숙련/일품) 처리
    // 플레이어가 상호작용하면 플레이어 인벤토리에 들어감(플레이어 인벤토리가 꽉찼다면 안들어감)
    /////////////////////////////////////
}
