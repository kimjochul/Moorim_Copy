using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Fridge : MonoBehaviour, IInteractable
{
    public InteractMode Mode { get; }
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return true;
    }

    public void Interact(PlayerController Player)
    {
    }

    public void OnInteractEnd(PlayerController player)
    {
    }

    public Vector3 GetInteractPosition()
    {
        return transform.position;
    }
}