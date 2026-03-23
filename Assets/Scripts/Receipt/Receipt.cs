using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
public class Receipt : MonoBehaviour
{
    private ReceiptData data;

    [SerializeField]
    private TextMeshProUGUI txtDay;
    [Header("명세표 항목들")]
    [SerializeField]
    private ReceiptSlot totalRevenue;
    [SerializeField]
    private ReceiptSlot fame;
    [SerializeField]
    private ReceiptSlot debt;
    [SerializeField]
    private ReceiptSlot happyGuests;
    [SerializeField]
    private ReceiptSlot angryGuests;
    [SerializeField]
    private ReceiptSlot netIncome;

    [Header("애니메이션 연출 시간")]
    [Range(0f, 3f)]
    [SerializeField]
    private float labelDelay;
    [Range(0f, 3f)]
    [SerializeField]
    private float valueDuration;
    [Range(0f, 3f)]
    [SerializeField]
    private float netIncomeDelay;
    [Range(0f, 3f)]
    [SerializeField]
    private float netIncomeDuration;
    [Range(0f, 3f)]
    [SerializeField]
    private float appendInterval;
    [Range(0f, 3f)]
    [SerializeField]
    private float lastAppendInterval;

    void Start()
    {
        StartAnimationText();
    }
    public void Init(ReceiptData data)
    {
        this.data = data;
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    public void StartAnimationText(UnityAction onComplete = null)
    {
        // 임시
        this.data = new ReceiptData
        {
            week = new DayData { day = 1, dayOfWeek = DayOfWeek.Monday},
            totalRevenue = 9517,
            fame = 1563,
            debt = 456,
            happyGuests = 12,
            angryGuests = 9,
            netIncome = 65178
        };

        ClearSlotText();

        this.txtDay.text = data.week.day + "일째";

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f);

        // Append: 애니메이션 / AppendInterval: 쉬는 시간 추가
        seq.Append(this.totalRevenue.AnimationText(this.data.totalRevenue, this.labelDelay, this.valueDuration));
        seq.AppendInterval(this.appendInterval);

        seq.Append(this.fame.AnimationText(this.data.fame, this.labelDelay, this.valueDuration));
        seq.AppendInterval(this.appendInterval);

        seq.Append(this.debt.AnimationText(this.data.debt, this.labelDelay, this.valueDuration));
        seq.AppendInterval(this.appendInterval);

        seq.Append(this.happyGuests.AnimationText(this.data.happyGuests, this.labelDelay, this.valueDuration));
        seq.AppendInterval(this.appendInterval);

        seq.Append(this.angryGuests.AnimationText(this.data.angryGuests, this.labelDelay, this.valueDuration));
        seq.AppendInterval(this.lastAppendInterval);

        seq.Append(this.netIncome.AnimationText(this.data.netIncome, this.netIncomeDelay, this.netIncomeDuration));

        seq.OnComplete(() => {
            // 전부 끝나면 할 행동
            onComplete?.Invoke();
        });
    }
    private void ClearSlotText()
    {
        this.txtDay.text = "";
        this.totalRevenue.ClearText();
        this.fame.ClearText();
        this.debt.ClearText();
        this.happyGuests.ClearText();
        this.angryGuests.ClearText();
        this.netIncome.ClearText();
    }
}
