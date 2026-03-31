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
    private Image imageBg;
    [SerializeField]
    private Sprite spriteEmpty;
    [SerializeField]
    private Sprite spriteFilled;

    [SerializeField]
    private Button btnBuy;

    public UnityAction<ItemData> onClickBuy;

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;

        this.itemName.text = this.itemData.itemName;
        this.amount.text = this.itemData.maxCapacity.ToString();
        this.price.text = this.itemData.price.ToString();

        this.btnBuy.onClick.RemoveAllListeners();

        this.btnBuy.onClick.AddListener(() =>
        {
            this.onClickBuy?.Invoke(this.itemData);
        });

        Background();
    }
    public void Clear()
    {
        this.onClickBuy = null;
        this.itemData = null;
        this.itemName.text = "";
        this.amount.text = "";
        this.price.text = "";

        Background();
    }
    private void Background()
    {
        if (this.itemData)
        {
            this.imageBg.sprite = this.spriteFilled;
            this.icon.gameObject.SetActive(true);
            this.btnBuy.interactable = true;
        }
        else
        {
            this.imageBg.sprite = this.spriteEmpty;
            this.icon.gameObject.SetActive(false);
            this.btnBuy.interactable = false;
        }
    }
}
