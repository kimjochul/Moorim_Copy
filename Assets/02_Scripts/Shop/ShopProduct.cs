using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

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

    public UnityAction<ItemData> onClickBuy;

    void Start()
    {
        this.btnBuy.onClick.AddListener(() =>
        {
            this.onClickBuy?.Invoke(this.itemData);
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
