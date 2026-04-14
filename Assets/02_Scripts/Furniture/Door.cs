using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public enum DoorSide
{
    LeftDoor,
    RightDoor
}

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorSide mySide;
    [SerializeField] private Transform leftDoorPart;   // 왼쪽 문 오브젝트
    [SerializeField] private Transform rightDoorPart;  // 오른쪽 문 오브젝트
    [SerializeField] private float smooth = 5f;

    public UnityAction onDoorOpened;
    public UnityAction onDoorClosed;

    private Transform movingPart;
    private Transform otherDoorPart;

    private bool isOpen = false;
    private bool isMoving = false;

    private Vector3 closePosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;
    public InteractMode Mode => InteractMode.Immediate;

    void Start()
    {
        Init();
    }

    void Update()
    {
        HandleMovement();
    }
    public void Init()
    {
        if (this.mySide == DoorSide.LeftDoor)
        {
            this.movingPart = this.leftDoorPart;
            this.otherDoorPart = this.rightDoorPart;
        }
        else
        {
            this.movingPart = this.rightDoorPart;
            this.otherDoorPart = this.leftDoorPart;
        }

        this.closePosition = this.movingPart.localPosition;

        this.openPosition = new Vector3(
            this.otherDoorPart.localPosition.x,
            this.closePosition.y,
            this.closePosition.z
        );

        this.targetPosition = this.isOpen ? this.openPosition : this.closePosition;
    }

    /// <summary>
    /// 외부에서 문의 열리는 방향을 실시간으로 바꿀 때 호출하는 함수
    /// </summary>
    public void ChangeDoorSide(DoorSide newSide)
    {
        if (this.isMoving || this.isOpen) return;

        this.mySide = newSide;
        Init();
    }

    private void HandleMovement()
    {
        if (!this.isMoving) return;

        this.movingPart.localPosition = Vector3.Lerp(
            this.movingPart.localPosition,
            this.targetPosition,
            Time.deltaTime * this.smooth
        );

        if (Vector3.Distance(this.movingPart.localPosition, this.targetPosition) < 0.01f)
        {
            this.movingPart.localPosition = this.targetPosition;
            this.isMoving = false;
        }
    }

    public bool CanInteract(Constants.PlayerType playerType)
    {
        return true;
    }

    public void Interact(PlayerController Player)
    {
        if (this.isMoving) return;

        this.isOpen = !this.isOpen;
        this.isMoving = true;

        this.targetPosition = this.isOpen ? this.openPosition : this.closePosition;

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