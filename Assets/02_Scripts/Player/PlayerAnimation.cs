using DG.Tweening;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Animator animator;

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashPickup = Animator.StringToHash("PickUp");
    private readonly int hashGive = Animator.StringToHash("Give");
    private readonly int hashInteract = Animator.StringToHash("Interact");

    void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.player.OnStateChanged += HandleStateChanged;
    }
    
    private void HandleStateChanged(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                DOTween.Kill(this.gameObject.GetInstanceID());

                float currentSpeed = this.animator.GetFloat(this.hashSpeed);

                DOVirtual.Float(currentSpeed, 0f, 0.1f, (value) =>
                {
                    this.animator.SetFloat(this.hashSpeed, value);
                }).SetId(this.gameObject.GetInstanceID());
                break;

            case PlayerState.Walk:
                this.animator.SetFloat(this.hashSpeed, 0.5f);
                break;

            case PlayerState.Run:
                this.animator.SetFloat(this.hashSpeed, 1f);
                break;

            case PlayerState.PickUp:
                this.animator.SetTrigger(this.hashPickup);
                break;

            case PlayerState.Give:
                this.animator.SetTrigger(this.hashGive);
                break;

            case PlayerState.Interact:
                this.animator.SetTrigger(this.hashInteract);
                break;
        }
    }

}
