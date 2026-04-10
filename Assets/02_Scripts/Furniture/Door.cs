using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform hinge;        // 회전 기준
    [SerializeField] private float openAngle = 90f;  // 문이 열렸을 때의 Y축 각도
    [SerializeField] private float closeAngle = 0f;  // 문이 닫혔을 때의 Y축 각도
    [SerializeField] private float smooth = 5f;      // 문이 열리고 닫히는 속도

    public UnityAction onDoorOpened; 
    public UnityAction onDoorClosed;

    private bool isOpen = false;
    private bool isMoving = false;

    private Camera mainCamera;
    private Quaternion targetRotation;

    public InteractMode Mode => InteractMode.ClickToInteract;

    void Start()
    {
        Init();
    }
    void Update()
    {
        HandleRotation();
    }
    public void Init()
    {
        this.mainCamera = Camera.main;

        this.targetRotation = Quaternion.Euler(0, this.closeAngle, 0);

        this.hinge.localRotation = this.targetRotation;
    }

    private void HandleRotation()
    {
        if (!this.isMoving) return;

        this.hinge.localRotation = Quaternion.Slerp(
            this.hinge.localRotation,
            this.targetRotation,
            Time.deltaTime * this.smooth
        );

        if (Quaternion.Angle(this.hinge.localRotation, this.targetRotation) < 0.1f)
        {
            this.hinge.localRotation = this.targetRotation;
            this.isMoving = false; 
        }
    }

    public bool CanInteract(Constants.PlayerType playerType)
    {
        return true;
    }

    public void Interact(PlayerController Player)
    {
        if (this.isMoving) { return; };

        this.isOpen = !this.isOpen;
        this.isMoving = true;

        float targetY = this.isOpen ? this.openAngle : this.closeAngle;
        this.targetRotation = Quaternion.Euler(0, targetY, 0);

        if (this.isOpen)
        {
            this.onDoorOpened?.Invoke();
        }
        else
        {
            this.onDoorClosed?.Invoke();
        }
    }

    public void OnInteractEnd(PlayerController player) { }

    public Vector3 GetInteractPosition()
    {
        return this.transform.position;
    }
}
