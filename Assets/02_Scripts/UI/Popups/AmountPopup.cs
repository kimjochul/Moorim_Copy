using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AmountPopup : PopupController
{
    private ItemData itemData;
    private RecipeData recipeData;

    [SerializeField]
    private TextMeshProUGUI amount;
    [SerializeField]
    private Button btnMinus;
    [SerializeField]
    private Button btnPlus;

    [SerializeField]
    private Button btnOk;
    [SerializeField]
    private Button btnCancel;

    private int currentAmount;
    private int maxAmount;

    void Start()
    {
        this.btnPlus.onClick.AddListener(OnClickPlus);
        this.btnMinus.onClick.AddListener(OnClickMinus);
    }
    public void Init(ItemData itemData)
    {
        this.itemData = itemData;
        this.maxAmount = itemData.maxCapacity;
        this.currentAmount = 1;

        UpdateUI();
    }
    
    public void Init(RecipeData recipeData)
    {
        this.recipeData = recipeData;
        this.maxAmount = 1;
        this.currentAmount = 1;

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
        this.btnCancel.onClick.AddListener(()=>
        {
            action?.Invoke();
            Destroy(this.gameObject);
        });
    }
}
