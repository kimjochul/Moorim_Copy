using System;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class CommonInventoryUIController : MonoBehaviour
{
    [SerializeField] Inventory meatInventory;
    [SerializeField] Inventory ingredientInventory;
    
    [SerializeField] private Button allButton;
    [SerializeField] private Button meatButton;
    [SerializeField] private Button vegetableButton;
    [SerializeField] private Button dairyButton;
    [SerializeField] private Button spicesButton;
    [SerializeField] private Button processedButton;

    [SerializeField] private Button sortButton;

    private InventoryFilter currentFilter = InventoryFilter.All;

    private void Awake()
    {
        allButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.All));
        meatButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.Meat));
        vegetableButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.Vegetable));
        dairyButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.Dairy));
        spicesButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.Spices));
        processedButton.onClick.AddListener(() => ChangeFilter(InventoryFilter.Processed));
        
        sortButton.onClick.AddListener(SortInventory);
    }

    public void ResetFilter()
    {
        ChangeFilter(InventoryFilter.All);
    }
    
    private void ChangeFilter(InventoryFilter filter)
    {
        currentFilter = filter;

        RefreshInventory(meatInventory, filter);
        RefreshInventory(ingredientInventory, filter);
    }   
    
    private void RefreshInventory(Inventory inventory, InventoryFilter filter)
    {
        foreach (Slot slot in inventory.Slots)
        {
            bool visible = IsVisible(slot, filter);
            slot.gameObject.SetActive(visible);
        }
    }

    private void SortInventory()
    {
        meatInventory.RefreshItemOrder(true);
        ingredientInventory.RefreshItemOrder(true);

        ChangeFilter(currentFilter);
    }
    
    private bool IsVisible(Slot slot, InventoryFilter filter)
    {
        if (slot.IsEmpty)
            return filter == InventoryFilter.All;

        if (slot.Item.Data is not IngredientData ingredient)
            return false;

        if (filter == InventoryFilter.All)
            return true;

        return ingredient.Filter == filter;
    }
}
