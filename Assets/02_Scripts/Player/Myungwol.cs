using UnityEngine;
using UnityEngine.Events;

public class Myungwol : PlayerController
{
    public UnityAction onPlaceFood;

    public override void OnSelected()
    {
        base.OnSelected();
    }
    public override void OnDeselected()
    {
        base.OnDeselected();
    }
}
