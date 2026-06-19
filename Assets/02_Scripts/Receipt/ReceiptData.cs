using System;
using UnityEngine;

[Serializable]
public struct ReceiptData
{
    public DayData week;             // 날짜
    public int totalRevenue;        // 전체 수익
    public int fame;                // 얻은 명성
    public int debt;                // 빚 
    public int happyGuests;         // 만족한 손님 수
    public int angryGuests;         // 불만족한 손님 수
    public int netIncome;           // 순수익
}
