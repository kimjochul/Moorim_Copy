using UnityEngine;
using static Constants;

public interface IInteractable
{
    //��ȣ�ۿ�
    void Interact(Inventory inventory = null);
    bool CanInteract(PlayerType playerType);
}
