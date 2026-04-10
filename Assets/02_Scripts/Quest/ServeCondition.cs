using UnityEngine;
using static Constants;

[CreateAssetMenu(menuName = "Quest/Condition/ServeGuest")]
public class ServeCondition : QuestCondition
{
    public GuestMood requiredGuestMood;
    public int requireCount;
    private int currentServeCount;
    
    public override void Init(Inventory inventory = null)
    {
        currentServeCount = 0;
    }

    public override void Notify(QuestEvent questEvent, GuestMood mood = GuestMood.Neutral)
    {
        if (questEvent.type != QuestEventType.ServeCustomer)
            return;

        if (mood < requiredGuestMood)
            return;
        
        currentServeCount += questEvent.currentCount;
        NotifyUpdated();
    }

    public override bool IsComplete()
    {
        return currentServeCount >= requireCount;
    }

    public override QuestCondition Clone()
    {
        var instance = CreateInstance<ServeCondition>();
        instance.requiredGuestMood = requiredGuestMood;
        instance.requireCount = requireCount;
        
        return instance;
    }

    public override string GetDescription()
    {
        string description = "";
        string mood = "";
        switch (requiredGuestMood)
        {
            case GuestMood.VeryBad:
                mood = "매우 나쁨";
                break;
            case GuestMood.Bad:
                mood = "나쁨";
                break;
            case GuestMood.Neutral:
                mood = "보통";
                break;
            case GuestMood.Good:
                mood = "좋음";
                break;
            case GuestMood.VeryGood:
                mood = "매우 좋음";
                break;
        }

        description = $"기분이 {mood} 이상인 손님 {requireCount}명 접객하기";
        return description;
    }

    public override string GetProgressText()
    {
        return $"{currentServeCount}/{requireCount}";
    }
}
