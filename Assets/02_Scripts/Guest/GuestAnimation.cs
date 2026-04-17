using System;
using System.Collections;
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

    public event Action OnSitDownFinished;
    public event Action OnStandUpFinished;
    public event Action OnHandUpFinished;
    public event Action OnHandDownFinished;
    public event Action OnAngryFinished;

    private Coroutine waitCoroutine;

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
        }
    }

    private void MoodChanged(GuestBase g, GuestMood mood)
    {
        // if (mood == GuestMood.VeryBad || mood == GuestMood.Bad)
        //     PlayAngry();
    }

    public void PlayWalk()
    {
        ResetAll();
        anim.SetBool(IS_WALKING, true);
    }

    public void PlayIdle()
    {
        ResetAll();
    }

    public void PlaySitDown()
    {
        StopWait();
        anim.SetBool(IS_WALKING, false);
        anim.SetTrigger(SITDOWN_TRIGGER);
        anim.SetBool(IS_SITTING, true);
        waitCoroutine = StartCoroutine(WaitForAnimation("SitDown", () =>
        {
            OnSitDownFinished?.Invoke();
        }));
    }

    public void PlaySit()
    {
        anim.SetBool(IS_SITTING, true);
    }

    public void PlayHandUp()
    {
       StopWait();
       anim.SetBool(IS_SITTING, true);
       anim.SetTrigger(HANDUP_TRIGGER);
       waitCoroutine = StartCoroutine(WaitForAnimation("HandUP", () =>
       {
           anim.SetBool(IS_EATING, true);
           OnHandUpFinished?.Invoke();
       }));
    }

    public void PlayHandDown()
    {
        StopWait();
        anim.SetBool(IS_EATING, false);
        anim.SetTrigger(HANDDOWN_TRIGGER);
        waitCoroutine = StartCoroutine(WaitForAnimation("HandDown", () =>
        {
            OnHandDownFinished?.Invoke();
        }));
    }

    public void PlayStandUp()
    {
        StopWait();
        anim.SetBool(IS_SITTING, false);
        anim.SetBool(IS_EATING, false);
        anim.SetBool(IS_ANGRY, false);
        anim.SetTrigger(STANDUP_TRIGGER);
        waitCoroutine = StartCoroutine(WaitForAnimation("StandUp", () =>
        {
            anim.SetBool(IS_WALKING, true);
            OnStandUpFinished?.Invoke();
        }));
    }

    public void PlayAngry()
    {
        StopWait();
        anim.SetBool(IS_ANGRY, true);
        // Angry 모션이 일정 시간 보여진 뒤 콜백
        waitCoroutine = StartCoroutine(WaitForAnimation("Angry", () =>
        {
            OnAngryFinished?.Invoke();
        }));
    }

    // ── 애니메이션 완료 대기 코루틴 ──

    private IEnumerator WaitForAnimation(string stateName, Action onComplete)
    {
        // 트리거가 반영될 때까지 1프레임 대기
        yield return null;

        // 해당 상태에 진입할 때까지 대기 (최대 1초 안전장치)
        float safety = 0f;
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            safety += Time.deltaTime;
            if (safety > 1f) break; // 상태 이름 불일치 시 무한루프 방지
            yield return null;
        }

        // 애니메이션 재생 완료 대기
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        while (info.IsName(stateName) && info.normalizedTime < 0.95f)
        {
            yield return null;
            info = anim.GetCurrentAnimatorStateInfo(0);
        }

        onComplete?.Invoke();
        waitCoroutine = null;
    }

    private void StopWait()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
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
