using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("캐릭터 이동 설정")]

    [Range(1f, 40f)]
    [SerializeField]
    private float moveSpeed = 10f;
    [Range(60f, 480f)]
    [SerializeField]
    private float angularSpeed = 120f;
    [Range(4f, 100f)]
    [SerializeField]
    private float accelerationSpeed = 8f;

    [SerializeField] 
    private PlayerController[] players;

    private IControllable selectedUnit;
    private Camera mainCamera;

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode) { }

    void Start()
    {
        this.mainCamera = Camera.main;

        if (this.players.Length > 0) 
            SelectPlayer(0);
    }

    void Update()
    {
        ClickToMovePlayer();
    }
    private void ClickToMovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.F1)) 
            SelectPlayer(0);
        if (Input.GetKeyDown(KeyCode.F2)) 
            SelectPlayer(1);

        if (Input.GetMouseButtonDown(0) && this.selectedUnit != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            this.selectedUnit.SetSpeed(this.moveSpeed, this.angularSpeed, this.accelerationSpeed);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                this.selectedUnit.MoveTo(hit.point);
            }
        }
    }
    private void SelectPlayer(int index)
    {
        if (index >= this.players.Length || this.players[index] == null) return;

        if (this.selectedUnit != null) 
            this.selectedUnit.OnDeselected();

        this.selectedUnit = this.players[index];
        this.selectedUnit.OnSelected();
    }
}
