using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 돈이나 명성 바뀔 때마다 UI 업데이트(action이 있다면 action에 추가하는 식으로 변경 가능)
/// </summary>
public class MoneyRepuTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI fameText;
    [SerializeField] private RestaurantEconomy restaurantEconomy;
    
    EconomyManager economyManager;
    
    private int currMoney;
    private int currFame;

    void Awake()
    {
        economyManager = FindFirstObjectByType<EconomyManager>();
        economyManager.TodayEconomyChanged += OnEconomyChanged;
    }

    private void OnEnable()
    {
        EconomyManager.Instance.RestaurantEconomyChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        EconomyManager.Instance.RestaurantEconomyChanged -= Refresh;
    }

    private void Refresh()
    {
        moneyText.text = EconomyManager.Instance.RestaurantEconomy.Money.ToString();
    }
    
    void Start()
    {
        if (restaurantEconomy == null)
        {
            Debug.LogError("Couldn't find RestuarantEconomy SO");
        }
        
        currMoney = restaurantEconomy.Money;
        currFame = restaurantEconomy.Fame;
        moneyText.text = currMoney.ToString();
        fameText.text = currFame.ToString();
    }

    public void OnEconomyChanged()
    {
        OnMoneyChanged();
        OnFameChanged();
    }
    
    public void OnMoneyChanged()
    {
        moneyText.text = economyManager.TodayMoneyIncome.ToString();
    }

    public void OnFameChanged()
    {
        fameText.text = economyManager.TodayFameIncome.ToString();
    }

    void OnDestroy()
    {
        economyManager.TodayEconomyChanged -= OnEconomyChanged;
    }
}
