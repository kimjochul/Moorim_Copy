using System;
using System.Collections;
using UnityEngine;
using static Constants;

public class ViolenceGuest : GuestBase
{
    [Header("난동 패널티")]
    public int badDamage = 2000;
    public int veryBadDamage = 3000;
    public float damageInterval = 2f; // 돈이 깎이는 주기 (초)

    [Header("난동 UI")]
    [SerializeField] private Sprite violenceSprite; // 난동 중 말풍선에 띄울 아이콘

    public bool isCausingTrouble = false;
    private Coroutine troubleCoroutine;

    private GuestBubbleUI bubble;
    
    public static event Action<ViolenceGuest> OnViolenceStarted;

    public override void ResetState()
    {
        base.ResetState();

        if (bubble == null)
            bubble = GetComponentInChildren<GuestBubbleUI>(true);

        isCausingTrouble = false;
        if (troubleCoroutine != null)
        {
            StopCoroutine(troubleCoroutine);
            troubleCoroutine = null;
        }
    }

    public override void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        base.Init(id, appearance, exit, manager);
        OnGuestMoodChanged += HandleViolenceMood;
    }

    private void HandleViolenceMood(GuestBase guest, GuestMood mood)
    {
        // 기분이 Bad 또는 VeryBad가 되면 난동 시작
        if ((mood == GuestMood.Bad || mood == GuestMood.VeryBad) && !isCausingTrouble)
        {
            StartTrouble(mood);
        }
    }

    private void StartTrouble(GuestMood mood)
    {
        isCausingTrouble = true;
    
        if (guestAnim != null)
        {
            guestAnim.PlayAngry(); // 난동 애니메이션
        }
    
        if (bubble != null)
            bubble.SetViolenceBubble(true, violenceSprite);

        InvokeOrderServed();   // 난동 시작하면 주문표에서 제거

        troubleCoroutine = StartCoroutine(TroubleRoutine(mood));
    
        OnViolenceStarted?.Invoke(this);
    }

    private IEnumerator TroubleRoutine(GuestMood mood)
    {
        while (isCausingTrouble)
        {
            int damageAmount = (mood == GuestMood.VeryBad) ? veryBadDamage : badDamage;
            
            EconomyManager.Instance.AddMoney(-damageAmount);
            Debug.Log($"<color=red>괴한이 난동을 피웁니다! 금전적 피해: -{damageAmount}</color>");
            
            yield return new WaitForSeconds(damageInterval);
        }
    }

    // 도둑의 CatchThief()와 같은 역할
    public void SuppressViolence()
    {
        if (!isCausingTrouble) return;

        Debug.Log("<color=cyan>괴한을 제압했습니다!</color>");
        isCausingTrouble = false;
        
        if (troubleCoroutine != null)
        {
            StopCoroutine(troubleCoroutine);
            troubleCoroutine = null;
        }

        if (bubble != null)
            bubble.SetViolenceBubble(false);

        // 제압 후 얌전히 일어나서 나가도록 처리
        isPlayingStandUp = true;
        if (guestAnim != null)
        {
            guestAnim.PlayStandUp(); 
        }
        else
        {
            ChangeState(GuestState.Exit);
        }
    }

    // --- 부모(GuestBase)의 자동 퇴장 타이머 차단 ---
    protected override void HandleWaitOrderState()
    {
        if (isCausingTrouble) return;
        base.HandleWaitOrderState();
    }

    protected override void HandleWaitFoodState()
    {
        if (isCausingTrouble) return;
        base.HandleWaitFoodState();
    }
}