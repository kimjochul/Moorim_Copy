using UnityEngine;
using static Constants;

public class RichManGuest : GuestBase
{
    public override void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        orderMinWaitLimit = 10f;
        orderMaxWaitLimit = 15f;
        foodMinWaitLimit = 15f; 
        foodMidWaitLimit = 45f; 
        foodMaxWaitLimit = 60f; 

        base.Init(id, appearance, exit, manager);
    }
}
