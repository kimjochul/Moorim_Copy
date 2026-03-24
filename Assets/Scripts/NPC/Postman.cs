using UnityEngine;

public class Postman : MonoBehaviour, IInteractable
{
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return false;
    }

    public void Interact()
    {
        
    }
}
