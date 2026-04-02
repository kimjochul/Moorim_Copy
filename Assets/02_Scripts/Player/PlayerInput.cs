using UnityEngine;
using UnityEngine.Events;
using static Constants;

public class PlayerInput : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        this.mainCamera = Camera.main;
    }

    void Update()
    {
        if (PlayerManager.Instance.SelectedUnit.CurrentState == PlayerState.Interact)
            return;

        PlayerSelection();
        MouseClick();
        InteractInput();
    }

    private void PlayerSelection()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            PlayerManager.Instance.SelectPlayer(PlayerType.Myungwol);
        else if (Input.GetKeyDown(KeyCode.F2))
            PlayerManager.Instance.SelectPlayer(PlayerType.Sowol);
    }
    private void InteractInput()
    {
        IControllable currentUnit = PlayerManager.Instance.SelectedUnit;

        if (currentUnit == null) return;

        if (Input.GetKeyDown(KeyCode.F) && currentUnit.CurrentState == PlayerState.Idle)
        {
            IInteractable target = PlayerManager.Instance.ActiveInteractable;
            Debug.Log(target);
            if (target != null)
            {
                PlayerType currentPlayer = PlayerManager.Instance.CurrentPlayer;
                if (target.CanInteract(currentPlayer))
                {
                    currentUnit.ChangeState(PlayerState.Interact);
                    Debug.Log(currentPlayer + " : " + target);
                    target.Interact();

                    PlayerManager.Instance.ReservedInteractable = null;
                    PlayerManager.Instance.ActiveInteractable = null;
                }
            }
        }
    }
    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    PlayerManager.Instance.ReservedInteractable = interactable;
                    PlayerManager.Instance.MoveSelectedUnitTo(interactable.GetInteractPosition());
                }
                else
                {
                    PlayerManager.Instance.ReservedInteractable = null;

                    if (PlayerManager.Instance.CurrentPlayer == PlayerType.Sowol) return;

                    Movement(hit.point);
                }
            }
        }
    }

    private void Movement(Vector3 destination)
    {
        PlayerManager.Instance.MoveSelectedUnitTo(destination);
    }
}