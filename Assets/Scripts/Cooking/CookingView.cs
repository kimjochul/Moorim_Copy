using System;
using UnityEngine;
using UnityEngine.UI;

public class CookingView : MonoBehaviour
{
    [SerializeField] private GameObject cookinPanel;
    [SerializeField] private Button cookBtn;
    [SerializeField] private Button closeBtn;

    // 조리 UI의 재료 슬롯칸 배열 / 프리팹
    //[SerializeField] private 


    // 현재 UI를 열게 한 조리대를 기억하는 변수
    private CookingStation cookingController;

    void Start()
    {
        ShowCookingUI(false);
        BindButtons();
    }

    private void BindButtons()
    {
        cookBtn.onClick.AddListener(OnStartCook);
        closeBtn.onClick.AddListener(() => ShowCookingUI(true));
    }

    // UI를 조리대에서 여닫게 할 메소드
    public void ShowCookingUI(bool active)
    {
        cookinPanel.SetActive(active);
    }


    // 창이 열리거나 요리가 완료된 후 레시피 비우기
    // 레시피가 선택되었을 때 슬롯들을 초기화해서 그려줌
    private void RenderRecipeSlost()
    {
        
    }


    // 인벤토리의 재료를 재료슬롯에 넣기 위해 외부에서 쓸 메소드

    // 조리 시작 버튼
    public void OnStartCook()
    {
        if (cookingController == null)
            return;
        
        // 조리대에게 요리를 시작하라고 명령
        ShowCookingUI(false);
    }
}
