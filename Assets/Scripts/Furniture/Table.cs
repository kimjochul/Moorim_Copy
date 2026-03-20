using UnityEngine;

public class Table : MonoBehaviour, ISeatable
{
    Transform ISeatable.GetAvailableSeatTransform()
    {
        return null;
    }

    bool ISeatable.HasEmptySeat()
    {
        return false;
    }

    void ISeatable.Sit()
    {
        
    }

    void ISeatable.StandUp()
    {
        
    }
}
