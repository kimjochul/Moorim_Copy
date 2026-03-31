using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recipeDescriptionText;
    [SerializeField] private Image _recipeIcon;
    [SerializeField] private Image _ordererIcon;
    [SerializeField] private List<Sprite> _guestIconSprites;
    
    private bool isShown = false;
    public bool IsShown => isShown;

    public void Init(GuestBase guest, RuntimeOrder order)
    {
        _ordererIcon.sprite = _guestIconSprites[(int)guest.myApperance];
        
        string descriptionText = "";
        bool isFirst = true;
        foreach (var recipe in order.item)
        {
            if (isFirst)
            {
                _recipeIcon.sprite = recipe.recipe.icon;
                isFirst = false;
            }
            
            // 2개 이상이면 수량 표시
            string recipeCountText = recipe.requredAmount >= 2 ? $" {recipe.requredAmount}개" : "";
            
            descriptionText += $"{recipe.recipe.recipeName} {recipeCountText}\n";
            descriptionText += $"- {recipe.recipe.cookingMethod}\n";
        }
        _recipeDescriptionText.text = descriptionText;
    }
}