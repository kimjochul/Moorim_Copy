using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeSlotUI slotPrefab;
    [SerializeField] private Transform content;

    [SerializeField] private UpgradeDetailPanel detailPanel;

    [SerializeField] private List<UpgradeData> upgradeList;

    private UpgradeSlotUI currentSelectedSlot;
    
    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var data in upgradeList)
        {
            var slot = Instantiate(slotPrefab, content);

            slot.Init(data, OnSelectUpgrade);
        }

        detailPanel.Clear();
    }

    private void OnSelectUpgrade(UpgradeData data, UpgradeSlotUI slot)
    {
        // 이전 선택 해제
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        // 현재 선택
        currentSelectedSlot = slot;
        currentSelectedSlot.SetSelected(true);
        
        detailPanel.Show(data);
    }
}
