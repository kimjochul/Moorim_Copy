using TMPro;
using UnityEngine;

/// <summary>
/// 돈이나 명성 바뀔 때마다 UI 업데이트(action이 있다면 action에 추가하는 식으로 변경 가능)
/// </summary>
public class MoneyRepuTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI fameText;

    EconomyManager economyManager;
    private int currMoney;
    private int currFame;
    
    void Start()
    {
        economyManager = FindFirstObjectByType<EconomyManager>();
        if (economyManager == null)
        {
            Debug.LogError("Couldn't find EconomyManager");
        }
            
        if (economyManager.RestaurantEconomy == null)
        {
            Debug.LogError("Couldn't find RestaurantEconomy");
        }
        currMoney = economyManager.RestaurantEconomy.Money;
        currFame = economyManager.RestaurantEconomy.Fame;
        moneyText.text = currMoney.ToString();
        fameText.text = currFame.ToString();
    }

    void Update()
    {
        if (currMoney != economyManager.RestaurantEconomy.Money)
        {
            OnMoneyChanged(economyManager.RestaurantEconomy.Money);
        }

        if (currFame != economyManager.RestaurantEconomy.Fame)
        {
            OnFameChanged(economyManager.RestaurantEconomy.Fame);
        }
    }
    
    public void OnMoneyChanged(int currentMoney)
    {
        moneyText.text = currentMoney.ToString();
    }

    public void OnFameChanged(int currentReputation)
    {
        fameText.text = currentReputation.ToString();
    }
}
