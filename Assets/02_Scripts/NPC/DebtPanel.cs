using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebtPanel : MonoBehaviour
{
    [SerializeField] private RestaurantEconomy restaurantEconomy;
    [SerializeField] private TextMeshProUGUI debtText;
    
    [SerializeField] private TMP_InputField amountInput;
    
    [SerializeField] private Button repayButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private NPCTalkPanel talkPanel;
    
    private int remainDebt;

    public event Action FullPayDebt;
    
    private void OnEnable()
    {
        repayButton.onClick.AddListener(RepaymentDebt);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        remainDebt = restaurantEconomy.Debt;

        amountInput.text = null;
        
        UpdateUI();
    }

    private void OnDisable()
    {
        repayButton.onClick.RemoveListener(RepaymentDebt);
        cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
    }
    
    private void UpdateUI()
    {
        string debtString = $"남은 빚 : {remainDebt}\n현재 소지금 : {restaurantEconomy.Money}";
        debtText.text = debtString;
    }
    
    private int GetInputAmount()
    {
        if (string.IsNullOrEmpty(amountInput.text))
            return 0;

        if (!int.TryParse(amountInput.text, out int amount))
            return 0;

        return Mathf.Max(0, amount);
    }
    
    private void RepaymentDebt()
    {
        int amount = GetInputAmount();

        if (amount <= 0)
        {
            talkPanel.ChangeText("얼마를 갚으실 거에요?");
            return;
        }

        if (!EconomyManager.Instance.CanSpend(amount))
        {
            talkPanel.ChangeText("그만큼 돈은 없으신 것 같은데요...");
            return;
        }
        
        bool isOverPay = amount > remainDebt;

        if (isOverPay)
        {
            amount = remainDebt;
        }
        else
        {
            talkPanel.ChangeText($"좋아요. {amount}만큼 갚으셨습니다.");
        }

        EconomyManager.Instance.SpendMoney(amount);
        remainDebt -= amount;
        EconomyManager.Instance.RepayDebt(amount);
        
        if (remainDebt <= 0)
        {
            if (isOverPay)
            {
                talkPanel.ChangeText($"후하게 주시네요. 하지만 {amount}원 정도면 충분합니다. 빚은 전부 갚으셨어요!");
            }
            else
            {
                talkPanel.ChangeText("빚을 전부 갚으셨네요. 훌륭합니다! 이제 빚은 없으세요");
            }
            FullPayDebt?.Invoke();
        }
        amountInput.text = null;
        UpdateUI();
    }

    private void OnCancelButtonClicked()
    {
        talkPanel.ChangeText("돈은 나중에 갚으세요");
        gameObject.SetActive(false);
    }
}
