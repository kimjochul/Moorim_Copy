using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable == PlayerManager.Instance.ReservedInteractable)
        {
            PlayerManager.Instance.ActiveInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null && PlayerManager.Instance.ActiveInteractable == interactable)
        {
            PlayerManager.Instance.ActiveInteractable = null;
        }
    }
}