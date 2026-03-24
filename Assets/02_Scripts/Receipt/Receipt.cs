using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
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

    // 명세표 연출
    private Sequence receiptSequence;

    void Awake()
    {
        Show();
        StartAnimationText();
    }
    public void Init(ReceiptData data)
    {
        this.data = data;
    }
    public void Show()
    {
        ClearSlotText();
    }
    public void Hide()
    {
        ClearSequence();
        ClearSlotText();
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

        this.txtDay.text = data.week.day + "일째";

        this.receiptSequence = DOTween.Sequence();

        this.receiptSequence.AppendInterval(0.5f);
      
        // Append: 애니메이션 / AppendInterval: 쉬는 시간 추가
        this.receiptSequence.Append(this.totalRevenue.AnimationText(this.data.totalRevenue, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);
  
        this.receiptSequence.Append(this.fame.AnimationText(this.data.fame, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.debt.AnimationText(this.data.debt, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);

        this.receiptSequence.Append(this.happyGuests.AnimationText(this.data.happyGuests, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.appendInterval);
   
        this.receiptSequence.Append(this.angryGuests.AnimationText(this.data.angryGuests, this.labelDelay, this.valueDuration));
        this.receiptSequence.AppendInterval(this.lastAppendInterval);
  
        this.receiptSequence.Append(this.netIncome.AnimationText(this.data.netIncome, this.netIncomeDelay, this.netIncomeDuration));
   
        this.receiptSequence.OnComplete(() => {
            // 전부 끝나면 할 행동
            this.receiptSequence = null;
            onComplete?.Invoke();
        });
    }
    private void ClearSequence()
    {
        this.receiptSequence.Kill();
        this.receiptSequence = null;
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
