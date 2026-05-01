using System;
using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class InventoryPopupController : PopupController
{
    [SerializeField] private bool isCommonInventory;

    public bool IsCommonInventory()
    {
        return isCommonInventory;
    }
}