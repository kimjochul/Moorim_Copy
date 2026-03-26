using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return false;
    }

    public void Interact(Inventory inventory = null)
    {
        
    }

    public Vector3 GetInteractPosition()
    {
        return default;
    }
}
