using System;
using UnityEngine;
using UnityEngine.UI;

public class CookingUIController : MonoBehaviour
{
    [SerializeField] private GameObject cookinUI;
    [SerializeField] private Button cookBtn;

    // 재료슬롯 // 추후 슬롯 갯수가 늘어날 수 있음 // 아마 조리대 레벨 기능이 생기면(리모델링) 올리지 않을까함

    // 현재 UI를 열게 한 조리대를 기억하는 변수
    private CookingStation cookingStation;

    void Start()
    {
        CloseCookingUI();
        BindButtons();
    }

    private void BindButtons()
    {
        cookBtn.onClick.AddListener(OnStartCook);
    }

    // UI를 조리대에서 열게할 메소드
    public void OpenCookingUI()
    {
        cookinUI.SetActive(true);
    }

    // UI 닫는 메소드
    public void CloseCookingUI()
    {
        cookinUI.SetActive(false);
    }

    // 창이 열리거나 요리가 완료된 후 슬롯 비우기 기능

    // 인벤토리의 재료를 재료슬롯에 넣기 위해 외부에서 쓸 메소드

    // 조리 시작 버튼
    public void OnStartCook()
    {
        if (cookingStation == null)
            return;
        
        // 조리대에게 요리를 시작하라고 명령
        CloseCookingUI();
    }
}
