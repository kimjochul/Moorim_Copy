using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Animator animator;

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashPickup = Animator.StringToHash("Pickup");
    private readonly int hashGive = Animator.StringToHash("Give");
    private readonly int hashIsInteracting = Animator.StringToHash("IsInteracting");

    private void Awake()
    {
        if (this.animator == null)
        {
            this.animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 플레이어의 이동 속도를 전달하여 걷기/뛰기
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        this.animator.SetFloat(this.hashSpeed, speed);
    }

    /// <summary>
    /// 아이템을 줍기
    /// </summary>
    public void PlayPickup()
    {
        this.animator.SetTrigger(this.hashPickup);
    }

    /// <summary>
    /// 아이템을 건네주기
    /// </summary>
    public void PlayGive()
    {
        this.animator.SetTrigger(this.hashGive);
    }

    /// <summary>
    /// 상호작용
    /// </summary>
    public void SetInteracting(bool isInteracting)
    {
        this.animator.SetBool(this.hashIsInteracting, isInteracting);
    }
}
