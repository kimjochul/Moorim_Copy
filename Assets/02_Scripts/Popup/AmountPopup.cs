using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AmountPopup : PopupController
{
    [SerializeField]
    private TextMeshProUGUI message;

    [SerializeField]
    private TextMeshProUGUI amount;
    [SerializeField]
    private Button btnMinus;
    [SerializeField]
    private Button btnPlus;
    [SerializeField]
    private TextMeshProUGUI totalPrice;

    [SerializeField]
    private Button btnOk;
    [SerializeField]
    private Button btnCancel;

    private bool isPrice;

    private int currentAmount;
    private int maxAmount;
    private int price;

    void Start()
    {
        this.btnPlus.onClick.AddListener(OnClickPlus);
        this.btnMinus.onClick.AddListener(OnClickMinus);
    }
    public void Init(string message, ItemData itemData, bool isPrice = false)
    {
        this.message.text = message;
        this.maxAmount = itemData.maxCapacity;
        this.price = itemData.price;
        this.currentAmount = 1;

        this.totalPrice.gameObject.SetActive(this.isPrice);

        UpdateUI();
    }
    // 증가
    private void OnClickPlus()
    {
        if (this.currentAmount < this.maxAmount)
        {
            this.currentAmount++;
            UpdateUI();
        }
    }
    // 감소
    private void OnClickMinus()
    {
        if (this.currentAmount > 1)
        {
            this.currentAmount--;
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        this.amount.text = this.currentAmount.ToString();
        this.totalPrice.text = (this.price * this.currentAmount).ToString();
    }
    public void ButtonOk(UnityAction action)
    {
        this.btnOk.onClick.AddListener(action);
    }
    public void ButtonCancel(UnityAction action)
    {
        this.btnCancel.onClick.AddListener(action);
    }
}
