using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPopupController : PopupController
{
    [SerializeField] TMP_Text _characterNameText;
    [SerializeField] TMP_Text _dialogText;
    [SerializeField] private float textSpeed = 0.2f;

    private string _currentFullDialog;
    public void Show(string characterName, string dialog)
    {
        base.Show();   
        _currentFullDialog = dialog;
        _characterNameText.text = characterName;
        _dialogText.text = "";
        StartCoroutine(ShowDialog(dialog));
    }

    // 마우스 클릭 시 전부 보여주기
    private void OnMouseClicked()
    {
        StopCoroutine(nameof(ShowDialog));
        _dialogText.text = _currentFullDialog;
    }

    public void OnClickConfirmButton()
    {
        Hide();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }

    IEnumerator ShowDialog(string dialog)
    {
        foreach (char c in dialog)
        {
            _dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
