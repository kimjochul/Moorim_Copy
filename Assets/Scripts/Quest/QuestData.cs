using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 각각의 퀘스트에 대한 데이터를 SO로 생성하기 위한 클래스
/// condition은 퀘스트 완료 조건에 대한 변수
/// </summary>
[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public int id;
    public string questName;
    public List<QuestCondition> conditions;
}

