using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public int id;
    public string questName;
    public List<RequireItem> requireItems;
}

[Serializable]
public class RequireItem
{
    public string questItem;
    public int requestCount;
}

// 개선 방향 여러 종류의 퀘스트가 가능하게 변경
// 필요한 퀘스트 아이템에 대한 내용을 string이 아닌 다른 타입으로 해서 유지 보수 효율적으로