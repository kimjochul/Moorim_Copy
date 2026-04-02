using UnityEngine;
using static Constants;

public interface IInteractable
{
    //��ȣ�ۿ�
    void Interact();
    bool CanInteract(PlayerType playerType);
    Vector3 GetInteractPosition();
}
