using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogPopupController : PopupController
{
    [SerializeField] TMP_Text _recipeNameText;
    [SerializeField] TMP_Text _recipeCountText;
    [SerializeField] TMP_Text _characterNameText;
    [SerializeField] TMP_Text _characterSpeciesText;
    [SerializeField] TMP_Text _characterJobText;
    [SerializeField] TMP_Text _dialogText;
    public void Show(Dictionary<Recipe, int> recipes/*, Guest guest*/)
    {
        _recipeNameText.text = recipes.recipeName;
        // 주문 음식 개수 가져오기
        _recipeCountText.text = 1.ToString();
        _characterJobText.text = 
        base.Show();        
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
