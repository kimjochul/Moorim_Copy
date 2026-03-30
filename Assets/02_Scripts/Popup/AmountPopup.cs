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
    private GameObject objPrice;

    [SerializeField]
    private Button btnOk;
    [SerializeField]
    private Button btnCancel;

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

        this.objPrice.SetActive(isPrice);

        UpdateUI();
    }
    // ����
    private void OnClickPlus()
    {
        if (this.currentAmount < this.maxAmount)
        {
            this.currentAmount++;
            UpdateUI();
        }
    }
    // ����
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
        
        int total = price * currentAmount;
        totalPrice.text = total.ToString();

        if (EconomyManager.Instance.CanSpend(total))
            totalPrice.color = Color.white;
        else
            totalPrice.color = Color.red;
    }
    public void ButtonOk(UnityAction<int> action)
    {
        this.btnOk.onClick.AddListener(() =>
        {
            action?.Invoke(this.currentAmount);

            Destroy(this.gameObject);
        });
    }
    public void ButtonCancel(UnityAction action)
    {
        this.btnCancel.onClick.AddListener(action);

        Destroy(this.gameObject);
    }
}
