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
    
    private bool isShown = false;
    public bool IsShown => isShown;

    public void Init(Order order)
    {
        // _ordererIcon.sprite = order.orderer.icon;
        string descriptionText = "";
        bool isFirst = true;
        foreach (var recipe in order.recipes)
        {
            if (isFirst)
            {
                _recipeIcon.sprite = recipe.Key.Data.icon;
                isFirst = false;
            }
            string recipeCountText = recipe.Value >= 2 ? $" {recipe.Value}개" : "";
            
            descriptionText += $"{recipe.Key.Data.name}{recipeCountText}\n";
            descriptionText += $"- {recipe.Key.Data.description}\n";
        }
        _recipeDescriptionText.text = descriptionText;
    }
}