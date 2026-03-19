using UnityEngine;
using TMPro;

public class DialogPopupController : PopupController
{
    // 회원가입 팝업에서 "확인" 버튼 클릭 시 동작할 함수를 전달받기 위한 델리게이트
    public delegate void OnSignupButtonClicked();
    private OnSignupButtonClicked _onSignupButtonClicked;

    public void Show(OnSignupButtonClicked onSignupButtonClicked)
    {
        _onSignupButtonClicked = onSignupButtonClicked;
        Show();        
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }
}
