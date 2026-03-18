using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("시작 설정")]
    [SerializeField]
    private float startOrbitRotationY = 45f;
    [SerializeField]
    private float startOrbitRotationX = 45f;
    [SerializeField]
    private float startHeight = 10;

    [Header("카메라 이동")]
    [SerializeField]
    private float moveSpeed = 10f;

    [Header("카메라 회전")]
    [SerializeField]
    private float orbitSpeed = 5f;
    private float orbitDistance = 10f;
    [SerializeField]
    private float minOrbitAngleY = 10f;
    [SerializeField]
    public float maxOrbitAngleY = 85f;

    [Header("카메라 줌")]
    [SerializeField]
    private float zoomSpeed = 10f;
    [SerializeField]
    private float minOrbitDistance = 2f;
    [SerializeField]
    private float maxOrbitDistance = 30f;

    [SerializeField]
    private Collider groundCollider;

    private Vector3 orbitTargetPoint;

    private float currentOrbitRotationY = 0f;
    private float currentOrbitRotationX = 0f;

    void Start()
    {
        CameraPositionInit();
    }
    void LateUpdate()
    {
        MoveCamera();
        ZoomCamera();
        OrbitMovement();
        ClampPositionToBounds();
        ApplyCameraTransform();
    }
    private void CameraPositionInit()
    {
        Vector3 center = this.groundCollider.bounds.center;
        this.orbitTargetPoint = center;

        this.currentOrbitRotationY = this.startOrbitRotationY;
        this.currentOrbitRotationX = this.startOrbitRotationX;

        float targetHeight = 10f;
        this.orbitDistance = targetHeight / Mathf.Sin(this.currentOrbitRotationY * Mathf.Deg2Rad);

        Quaternion rotation = Quaternion.Euler(this.currentOrbitRotationY, this.currentOrbitRotationX, 0f);
        this.transform.position = center + rotation * Vector3.back * this.orbitDistance;
        this.transform.LookAt(center);
    }
    private void MoveCamera()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = this.transform.forward;
        Vector3 right = this.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v + right * h).normalized;

        this.orbitTargetPoint += moveDir * this.moveSpeed * Time.deltaTime;
    }
    private void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            this.orbitDistance -= scroll * this.zoomSpeed;
            this.orbitDistance = Mathf.Clamp(this.orbitDistance, this.minOrbitDistance, this.maxOrbitDistance);
        }
    }
    private void OrbitMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 rot = this.transform.localRotation.eulerAngles;
            this.currentOrbitRotationY = rot.x;
            this.currentOrbitRotationX = rot.y;
        }
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * this.orbitSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * this.orbitSpeed;

            this.currentOrbitRotationX += mouseX;
            this.currentOrbitRotationY -= mouseY;

            this.currentOrbitRotationY = Mathf.Clamp(this.currentOrbitRotationY, this.minOrbitAngleY, this.maxOrbitAngleY);
        }
    }
    private void ClampPositionToBounds()
    {
        if (this.groundCollider == null) return;

        Vector3 currentPos = this.orbitTargetPoint;
        Bounds bounds = this.groundCollider.bounds;

        currentPos.x = Mathf.Clamp(currentPos.x, bounds.min.x, bounds.max.x);
        currentPos.z = Mathf.Clamp(currentPos.z, bounds.min.z, bounds.max.z);

        this.orbitTargetPoint = currentPos;
    }
    private void ApplyCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(this.currentOrbitRotationY, this.currentOrbitRotationX, 0f);
        this.transform.position = this.orbitTargetPoint + rotation * Vector3.back * this.orbitDistance;
        this.transform.LookAt(this.orbitTargetPoint);
    }
}
