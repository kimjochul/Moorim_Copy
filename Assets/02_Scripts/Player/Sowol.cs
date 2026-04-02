using System;
using Unity.VisualScripting;
using UnityEngine;

public class Sowol : PlayerController
{
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
        CheckInteractState();
    }

    private void CheckInteractState()
    {
        if(CurrentState == PlayerState.Interact && cookingStation.CurrentState != CookingState.Cooking)
            ChangeState(PlayerState.Idle);
    }
}
