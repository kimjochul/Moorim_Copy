using UnityEngine;
using static Constants;

public class ViolenceGuest : GuestBase
{
    [Header("난동 패널티")]
    public int badDamage = 2000;
    public int veryBadDamage = 3000;

    // 패널티가 중복으로 들어가는 것을 막기 위한 플래그
    private bool hasAppliedBadDamage = false;
    private bool hasAppliedVeryBadDamage = false;

    public override void ResetState()
    {
        base.ResetState();
        
        // 풀링으로 다시 스폰될 때 플래그 초기화
        hasAppliedBadDamage = false;
        hasAppliedVeryBadDamage = false;
    }

    public override void Init(int id, GuestAppearance appearance, Transform exit, GuestManager manager)
    {
        base.Init(id, appearance, exit, manager);
        
        // GuestBase.ResetState()에서 이벤트가 모두 초기화(null)되므로, Init에서 다시 구독해줍니다.
        OnGuestMoodChanged += HandleViolenceMood;
    }

    private void HandleViolenceMood(GuestBase guest, GuestMood mood)
    {
        if (mood == GuestMood.Bad && !hasAppliedBadDamage)
        {
            ApplyDamage(badDamage);
            hasAppliedBadDamage = true;
        }
        else if (mood == GuestMood.VeryBad && !hasAppliedVeryBadDamage)
        {
            // 음식 품질 문제 등으로 Bad를 거치지 않고 바로 VeryBad가 되었을 경우를 대비한 안전장치
            if (!hasAppliedBadDamage)
            {
                ApplyDamage(badDamage);
                hasAppliedBadDamage = true;
            }

            ApplyDamage(veryBadDamage);
            hasAppliedVeryBadDamage = true;
        }
    }

    private void ApplyDamage(int damageAmount)
    {
        if (damageAmount > 0)
        {
            // 돈을 떨어트리기 위해 AddMoney에 음수를 전달합니다.
            EconomyManager.Instance.AddMoney(-damageAmount);
            Debug.Log($"<color=red>괴한이 난동을 피웁니다! 금전적 피해: -{damageAmount}</color>");
        }
    }
}