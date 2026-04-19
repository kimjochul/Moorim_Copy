using UnityEngine;
using static Constants;

public class RichManGuest : GuestBase
{
    public override void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        orderMinWaitLimit = 10f;
        orderMaxWaitLimit = 15f;
        foodMinWaitLimit = 30f; 
        foodMaxWaitLimit = 45f; 

        base.Init(id, appearance, exit, manager);
    }
}
