using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀘스트 종류
/// </summary>
public enum QuestType
{
    None, // null 대신 쓸 타입
    Merchant, // 상인이 주는 퀘스트
    PostMan, // 우체부가 주는 퀘스트
    Story
}

/// <summary>
/// 퀘스트 보상 클래스
/// </summary>
[Serializable]
public class QuestReward
{
    public int gold;
    public int reputation;
}

/// <summary>
/// 각각의 퀘스트에 대한 데이터를 SO로 생성하기 위한 클래스
/// condition은 퀘스트 완료 조건에 대한 변수
/// </summary>
[CreateAssetMenu(fileName = "QuestData", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int id;
    public QuestType questType;
    public string questName;
    public List<QuestCondition> conditions;
    public QuestReward reward;
}

