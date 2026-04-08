using System.Data;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(Animator))]
public class GuestAnimation : MonoBehaviour
{
    private Animator anim;
    private GuestBase guest;
    
    // 오타 방지 및 메모리 절약
    // static - 모든 손님 인스턴스가 같은 값을 공유 / readonly - 한 번 설정되면 변경 불가(실수로 값 바꾸는 걸 방지)
    // stirngToHash = answkduf -> 정수 변환을 최초 1회만 수행
    private static readonly int IS_WALKING = Animator.StringToHash("IsWalking");
    private static readonly int IS_SITTING = Animator.StringToHash("IsSitting");
    private static readonly int IS_EATING = Animator.StringToHash("IsEating");
    private static readonly int IS_ANGRY = Animator.StringToHash("IsAngry");
    private static readonly int SITDOWN_TRIGGER = Animator.StringToHash("SitDown");
    private static readonly int STANDUP_TRIGGER = Animator.StringToHash("StandUp");
    private static readonly int HANDUP_TRIGGER = Animator.StringToHash("HandUp");
    private static readonly int HANDDOWN_TRIGGER = Animator.StringToHash("HandDown");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Bind(GuestBase target)
    {
        guest = target;
        guest.OnGuestStateChanged += StateChanged;
        guest.OnGuestMoodChanged += MoodChanged;
    }

    private void StateChanged(GuestBase g, GuestState state)
    {
        switch (state)
        {
            case GuestState.Enter:      PlayWalk();         break;
            case GuestState.Waiting:    PlayIdle();         break;
            case GuestState.WaitOrder:  PlaySitDown();      break;
            case GuestState.WaitFood:                       break;
            case GuestState.Eat:        PlayHandUp();       break;
            case GuestState.Pay:        PlayHandDown();     break;
            case GuestState.Exit:       PlayStandUp();      break;
        }
    }

        private void MoodChanged(GuestBase g, GuestMood mood)
    {
        if (mood == GuestMood.VeryBad || mood == GuestMood.Bad)
            PlayAngry();
    }

    private void PlayWalk()
    {
        ResetAll();
        anim.SetBool(IS_WALKING, true);
    }

    private void PlayIdle()
    {
        ResetAll();
    }

    private void PlaySitDown()
    {
        anim.SetBool(IS_WALKING, false);
        anim.SetTrigger(SITDOWN_TRIGGER);
        anim.SetBool(IS_SITTING, true);
    }

    private void PlayStandUp()
    {
        ResetAll();
        anim.SetTrigger(STANDUP_TRIGGER);
        anim.SetBool(IS_WALKING, true);
    }

    private void PlaySit()
    {
        ResetAll();
        anim.SetBool(IS_SITTING, true);
    }

    private void PlayEat()
    {
        ResetAll();
        anim.SetTrigger(HANDUP_TRIGGER);
        anim.SetBool(IS_EATING, true);
    }

    private void PlayHandUp()
    {
       ResetAll();
       anim.SetBool(IS_SITTING, true);
       anim.SetTrigger(HANDUP_TRIGGER);
    }

    private void PlayHandDown()
    {
        ResetAll();
        anim.SetBool(IS_SITTING, true);
        anim.SetTrigger(HANDDOWN_TRIGGER);
    }

    private void PlayAngry()
    {
        ResetAll();
        anim.SetBool(IS_ANGRY, true);
    }

    // pool 재사용시 초기화
    public void ResetAnimation()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        ResetAll();
    }

    private void ResetAll()
    {
        anim.SetBool(IS_WALKING, false);
        anim.SetBool(IS_SITTING, false);
        anim.SetBool(IS_EATING, false);
        anim.SetBool(IS_ANGRY, false);
        anim.ResetTrigger(SITDOWN_TRIGGER);
        anim.ResetTrigger(STANDUP_TRIGGER);
        anim.ResetTrigger(HANDUP_TRIGGER);
        anim.ResetTrigger(HANDDOWN_TRIGGER);
    }
}
