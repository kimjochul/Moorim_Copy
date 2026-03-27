using UnityEngine;
using static Constants;

public class PlayerInteractor : MonoBehaviour
{
    private PlayerController playerController;
    private IInteractable currentTarget;

    private void Awake()
    {
        this.playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (this.currentTarget != null && this.currentTarget != PlayerManager.Instance.ReservedInteractable)
            this.currentTarget = null;

        if (this.playerController.CurrentState != PlayerState.Idle) return;

        // F키를 누르면 상호작용 시작
        if (this.currentTarget != null && Input.GetKeyDown(KeyCode.F))
        {
            Interaction(this.currentTarget);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable == PlayerManager.Instance.ReservedInteractable)
        {
            this.currentTarget = interactable;
            // UI 켜기
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable != null && this.currentTarget == interactable)
        {
            this.currentTarget = null;
            // UI 끄기
        }
    }
    private void Interaction(IInteractable target)
    {
        PlayerType currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (target.CanInteract(currentPlayer))
        {
            this.playerController.ChangeState(PlayerState.Interact);

            target.Interact();
        }
    }
}