using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
public class Receipt : MonoBehaviour
{
    private ReceiptData data;

    [SerializeField] private DayManager dayManager;
    [SerializeField] private GuestResultTracker guestTracker;

    [SerializeField] private TextMeshProUGUI txtDay;

    [Header("명세표 항목들")]
    [SerializeField] private ReceiptSlot totalRevenue;
    [SerializeField] private ReceiptSlot fame;
    [SerializeField] private ReceiptSlot debt;
    [SerializeField] private ReceiptSlot happyGuests;
    [SerializeField] private ReceiptSlot angryGuests;
    [SerializeField] private ReceiptSlot netIncome;

    [SerializeField] private GameObject objStamp;
    [SerializeField] private Button btnConfirm;

    [Header("애니메이션 연출 시간")]
    [Range(0f, 3f)][SerializeField] private float labelDelay;
    [Range(0f, 3f)][SerializeField] private float valueDuration;
    [Range(0f, 3f)][SerializeField] private float netIncomeDelay;
    [Range(0f, 3f)][SerializeField] private float netIncomeDuration;
    [Range(0f, 3f)][SerializeField] private float appendInterval;
    [Range(0f, 3f)][SerializeField] private float lastAppendInterval;

    // 명세표 연출
    private Sequence receiptSequence;

    void Awake()
    {
        Hide();
    }

    void OnEnable()
    {
        if (this.guestTracker != null)
        {
            this.guestTracker.OnDayResultReady += HandleDayResult;
        }
    }

    void OnDisable()
    {
        if (this.guestTracker != null)
        {
            this.guestTracker.OnDayResultReady -= HandleDayResult;
        }
    }

    private void HandleDayResult(GuestResultData dailyData, GuestResultData totalData)
    {
        this.data = new ReceiptData
        {
            week = this.dayManager.DayData,
            totalRevenue = EconomyManager.Instance.TodayMoneyIncome,
            fame = EconomyManager.Instance.TodayFameIncome,
            debt = 456, // 나중에 빚 데이터로 변경

            happyGuests = dailyData.goodCount + dailyData.veryGoodCount,
            angryGuests = dailyData.badCount + dailyData.veryBadCount,

            netIncome = 1 // 나중에 순 수익 데이터로 변경
        };

        Show();
        StartAnimationText();
    }

    public void Show()
    {
        ClearSlotText();

        if (this.objStamp != null)
            this.objStamp.SetActive(false);
        if (this.btnConfirm != null)
            this.btnConfirm.gameObject.SetActive(false);
    }

    public void Hide()
    {
        ClearSequence();
        ClearSlotText();
    }

    public void StartAnimationText(UnityAction onComplete = null)
    {
        this.txtDay.text = data.week.day + "일째";

        this.receiptSequence = DOTween.Sequence();
        this.receiptSequence.AppendInterval(0.5f);

        this.receiptSequence.Append(this.totalRevenue.AnimationText(this.data.totalRevenue, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.fame.AnimationText(this.data.fame, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.happyGuests.AnimationText(this.data.happyGuests, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.debt.AnimationText(this.data.debt, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.angryGuests.AnimationText(this.data.angryGuests, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.lastAppendInterval);

        this.receiptSequence.Append(this.netIncome.AnimationText(this.data.netIncome, this.netIncomeDelay, this.netIncomeDuration));

        this.receiptSequence.OnComplete(() => {
            this.receiptSequence = null;
            RectTransform rt = this.objStamp.GetComponent<RectTransform>();

            UIAnimationManager.Instance.ShowStamp(rt,
                onStart: () =>
                {
                    if (this.objStamp != null)
                        this.objStamp.SetActive(true);
                },
                onComplete: () =>
                {
                    if (this.btnConfirm != null)
                        this.btnConfirm.gameObject.SetActive(true);
                    onComplete?.Invoke();
                }
            );
        });
    }

    private void ClearSequence()
    {
        if (this.receiptSequence != null)
        {
            this.receiptSequence.Kill();
            this.receiptSequence = null;
        }
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
