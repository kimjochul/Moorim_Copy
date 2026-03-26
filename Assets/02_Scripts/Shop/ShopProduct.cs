using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopProduct : MonoBehaviour
{
    private ItemData itemData;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI amount;
    [SerializeField]
    private TextMeshProUGUI price;

    [SerializeField]
    private Button btnBuy;

    void Start()
    {
        this.btnBuy.onClick.AddListener(() =>
        {
            UIManager.Instance.CreateAmountPopup("구매하시겠습니까?", this.itemData, true);
        });
    }

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;

        this.itemName.text = this.itemData.itemName;
        this.amount.text = this.itemData.maxCapacity.ToString();
        this.price.text = this.itemData.price.ToString();
    }
    
}
