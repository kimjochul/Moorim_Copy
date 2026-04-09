using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/ServeGuest")]
public class ServeCondition : QuestCondition
{
    
    
    
    public override void Init(Inventory inventory)
    {
        
    }

    public override void Notify(QuestEvent questEvent)
    {
        
    }

    public override bool IsComplete()
    {
        return true;
    }

    public override QuestCondition Clone()
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string GetProgressText()
    {
        throw new System.NotImplementedException();
    }
}
