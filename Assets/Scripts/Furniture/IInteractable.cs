using UnityEngine;
using static Constants;

public interface IInteractable
{
    //상호작용
    void Interact(IControllable player);
    bool CanInteract(PlayerType playerType);
}
