using UnityEngine;

public class RichManGuest : GuestBase
{
    void Awake()
    {
        orderWaitLimit = 15f;
        foodMinWaitLimit = 15f; 
        foodMidWaitLimit = 45f; 
        foodMaxWaitLimit = 60f; 
    }

    protected override int CalculateFinalPrice()
    {
        basePrise = 0;

        if (myOrder != null && myOrder.orderedFood != null)
            basePrise = myOrder.orderedFood.price;

        float richMultiplier = 1.0f;

        switch (currentMood)
        {
            case Constants.GuestMood.VeryBad: 
                richMultiplier = 0.1f; // 기분 상하면 10%만 던져주고 감
                break; 
            case Constants.GuestMood.Bad: 
                richMultiplier = 0.5f; 
                break;
            case Constants.GuestMood.Neutral: 
                richMultiplier = 1.5f; // 평범해도 기본적으로 팁을 조금 더 줌
                break;
            case Constants.GuestMood.Good: 
                richMultiplier = 3.0f; // 기분 좋으면 3배
                break;
            case Constants.GuestMood.VeryGood: 
                richMultiplier = 5.0f; // 엄청 좋으면 5배
                break; 
        }
        
        return Mathf.RoundToInt(basePrise * richMultiplier);
    }
}
