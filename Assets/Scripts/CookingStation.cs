using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    // 조리 UI : UI용 스크립트가 들어간 오브젝트를 연결
    // 완성될 요리가 가야할 배식구 : 배식구용 스크립트가 들어간 오브젝트를 연결

    [SerializeField] private float cookTime = 15f;

    private bool isCooking = false;

    // 플레이어가 조리대에 상호작용했을 때 실행되어야할 기능
    private void Interact()
    {
        if (isCooking)
        {
            Debug.Log("보글보글.. 현재 조리 중입니다!");
            return;
        }

        // UI 오픈
    }

    // 조리 버튼이 눌렸을 때 실행되어야할 기능
    private void StartCooking()
    {
        isCooking = true;
        

        StartCoroutine(CookingRoutine());
    }

    private IEnumerator CookingRoutine()
    {
        yield return new WaitForSeconds(cookTime);

        isCooking = false;
        Debug.Log("요리 완성!");

        // 배식구로 보냄
    }
    //// 요리가 되는 동안 해야하는 기능 ////
    // 15초 타이머
    // UI 비활성화
    // 냄비 애니메이션, SFX 실행
    // 15초 끝날경우
    // 플레이어 데이터(요리 횟수)를 읽어서 음식 퀄리티(보통/숙련/일품) 처리
    // 배식구로 이동

    // 제안 넣어볼만한 것 : 소월이 완성된 냄비를 다시 상호작용해야 배식구로 넘어감. 완성된 음식이 10초 이상 방치되면 타면서 없어짐
    /////////////////////////////////////
}
