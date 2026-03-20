using UnityEngine;

public interface ISeatable
{
    bool HasEmptySeat();
    Transform GetAvailableSeatTransform();
    void Sit();
    void StandUp();
}
