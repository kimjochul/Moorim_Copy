using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class GuestBubbleUI : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] private GameObject moodBubble;
    [SerializeField] private RectTransform moodRect;
    [SerializeField] private GameObject statusBubble;
    [SerializeField] private RectTransform statusRect;
    

    [Header("Bubble UI")]
    [SerializeField] private Image moodIcon;
    [SerializeField] private Image statusIcon;

    [Header("Mood UI")]
    [SerializeField] private Sprite veryBadSprite;
    [SerializeField] private Sprite badSprite;
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite goodSprite;
    [SerializeField] private Sprite veryGoodSprite;

    [Header("State UI")]
    [SerializeField] private Sprite orderSprite;
    // [SerializeField] private Sprite foodSprite;
    [SerializeField] private Sprite paySprite;

    private GuestBase guest;

    void Awake()
    {
        moodRect = moodBubble.GetComponent<RectTransform>();
        statusRect = statusBubble.GetComponent<RectTransform>();
    }

    public void Bind(GuestBase target)
    {
        guest = target;

        // 상태 변경 이벤트 구독
        guest.OnGuestStateChanged += BubbleChanged;

        // 기분 변경 이벤트 구독
        guest.OnGuestMoodChanged += MoodChanged;

        Debug.Log($"[Bind] OnGuestMoodChanged 구독 완료");

        // 메뉴를 골랐을 때
        guest.OnGuestReadyToOrder += ReadyToOrder;

        // 주문 접수됐을 때
        guest.OnOrderPlaced += OrderPlaced;

        // 계산 중일 때
        guest.OnGuestReadyToPay += ReadyToPay;
    }

    private void BubbleChanged(GuestBase g, GuestState state)
    {
        switch (state)
        {
            case GuestState.WaitOrder:
                // 착석 → 기분 말풍선 켜기
                ShowBubble(moodBubble, moodRect);
                moodIcon.sprite = GetMoodSprite(g.CurrentMood);
                break;
            case GuestState.WaitFood:
                // 기분 유지, 상태 말풍선은 OrderPlaced에서 이미 켜짐
                break;
            case GuestState.Eat:
                // 기분 유지, 상태 말풍선 끄기 (음식 받았으니까)
                HideBubble(statusBubble, statusRect);
                break;
            case GuestState.Pay:
                // 기분 말풍선 끄고 계산 아이콘으로 교체
                moodBubble.SetActive(false);
                break;
            case GuestState.Exit:
                // 전부 끄기
                // moodBubble.SetActive(false);
                HideBubble(statusBubble, statusRect);
                break;
        }
    }

    private void MoodChanged(GuestBase g, GuestMood mood)
    {
        moodIcon.sprite = GetMoodSprite(mood);
    }

    private void ReadyToOrder(GuestBase g)
    {
        // 메뉴를 골랐을 때 주문표 아이콘 표시
        statusBubble.SetActive(true);
        statusIcon.sprite = orderSprite;
    }

    private void OrderPlaced(GuestBase g, RuntimeOrder order)
    {
        // 주문 접수 → 음식 아이콘으로 변경
        if (order.recipe != null && order.recipe.icon != null)
        {
            ShowBubble(statusBubble, moodRect);
            statusIcon.sprite = order.recipe.icon;
        }
    }

    private void ReadyToPay(GuestBase g)
    {
        // 계산 중일 때 아이콘 표시
        statusBubble.SetActive(true);
        statusIcon.sprite = paySprite;
    }

    // 전부 끄기 (ResetState에서 호출용)
    public void HideAll()
    {
        moodBubble.SetActive(false);
        statusBubble.SetActive(false);
    }

    private Sprite GetMoodSprite(GuestMood mood)
    {
        switch (mood)
        {
            case GuestMood.VeryGood: return veryGoodSprite;
            case GuestMood.Good:     return goodSprite;
            case GuestMood.Neutral:  return neutralSprite;
            case GuestMood.Bad:      return badSprite;
            case GuestMood.VeryBad:  return veryBadSprite;
            default:                 return neutralSprite;
        }
    }

    
    private void ShowBubble(GameObject bubble, RectTransform rect)
    {
        bubble.SetActive(true);
        UIAnimationManager.Instance.ShowPop(rect);
    }

    private void HideBubble(GameObject bubble, RectTransform rect)
    {
        UIAnimationManager.Instance.HidePop(rect);
        bubble.SetActive(false);
    }

}
