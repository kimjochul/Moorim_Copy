using TMPro;
using UnityEngine;

public class UpgradeCostUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI countText;

    public void Init(UpgradeCost cost)
    {
        bool enough;

        if (cost.money > 0)
        {
            itemNameText.text = "돈";
            int currentMoney = EconomyManager.Instance.RestaurantEconomy.Money;
            countText.text = $"{currentMoney}/{cost.money}";
            enough = currentMoney >= cost.money;
        }
        else
        {
            ItemData itemData = DataManager.Instance.GetItemData(cost.itemID);
            itemNameText.text = itemData.itemName;
            int currentCount = InventoryManager.Instance.GetInventory(Constants.InventoryType.Fridge).GetItemCount(cost.itemID);
            countText.text = $"{currentCount}/{cost.count}";
            enough = currentCount >= cost.count;
        }
        countText.color = enough ? Color.white : Color.red;
    }
}
