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
        PlayerSelection();
        MouseClick();
    }

    private void PlayerSelection()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            PlayerManager.Instance.SelectPlayer(PlayerType.Myungwol);
        else if (Input.GetKeyDown(KeyCode.F2))
            PlayerManager.Instance.SelectPlayer(PlayerType.Sowol);
    }

    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

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