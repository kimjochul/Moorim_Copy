using UnityEngine;

public class BondItemTab : BaseTab
{
    [SerializeField] private Inventory bondInventory;
    [SerializeField] private BondItemDetailPanel detailPanel;
    
    public override void SetupData()
    {
        bondInventory = InventoryManager.Instance.GetInventory(Constants.InventoryType.BondItem);

        foreach (var slot in bondInventory.Slots)
        {
            slot.OnSlotClick += OnClickSlot;
        }
    }

    public override void ResetSetting()
    {
        detailPanel.gameObject.SetActive(false);
    }
    
    private void OnClickSlot(Slot slot)
    {
        if (slot.IsEmpty)
        {
            detailPanel.gameObject.SetActive(false);
            return;
        }
        detailPanel.gameObject.SetActive(true);
        detailPanel.SetItem(slot.Item);
    }
}
