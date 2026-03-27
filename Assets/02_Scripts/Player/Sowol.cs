using System;
using UnityEngine;

public class Sowol : PlayerController
{
    [SerializeField] private Inventory myInventory;
    [SerializeField] private CookingStation cookingStation;
    
    public override void OnSelected()
    {
        base.OnSelected();
    }
    public override void OnDeselected()
    {
        base.OnDeselected();
    }

    private void LateUpdate()
    {
        GetFoodItem();
    }

    private void GetFoodItem()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cookingStation.Interact(myInventory);
        }
    }
}
