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
        {
            PlayerManager.Instance.SelectPlayer(PlayerType.Myungwol);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerManager.Instance.SelectPlayer(PlayerType.Sowol);
        }
    }
    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                // 오브젝트가 상호작용 가능한 사물인지 확인
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)   //상호 작용
                {
                    PlayerManager.Instance.MoveSelectedUnitTo(interactable.GetInteractPosition(), () =>
                    {
                        Interaction(interactable);
                    });
                }
                else    // 이동
                {
                    if (PlayerManager.Instance.CurrentPlayer == PlayerType.Sowol)
                        return;

                    Movement(hit.point);
                }
            }
        }
    }
    private void Interaction(IInteractable target)
    {
        PlayerType currentPlayer = PlayerManager.Instance.CurrentPlayer;

        if (target.CanInteract(currentPlayer))
        {
            target.Interact();
        }
    }
    private void Movement(Vector3 destination)
    {
        PlayerManager.Instance.MoveSelectedUnitTo(destination);
    }
}
