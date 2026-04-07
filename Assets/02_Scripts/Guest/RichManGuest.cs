using UnityEngine;

public class RichManGuest : GuestBase
{
    void Awake()
    {
        orderMinWaitLimit = 10f;
        orderMaxWaitLimit = 15f;
        foodMinWaitLimit = 15f; 
        foodMidWaitLimit = 45f; 
        foodMaxWaitLimit = 60f; 
    }
}
