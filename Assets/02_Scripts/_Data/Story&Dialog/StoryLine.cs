using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryLine
{
    public int id;
    
    public LineType lineType;
    
    public int characterID;
    public Emotion emotion; 
    public MovingInStoryUI movingInStoryUI;
    
    [TextArea(3, 10)]
    public string line;
    
    public bool isInnerThought;
}

public enum LineType
{
    Dialogue,   // 일반 대화
    Narration   // 나레이션
}

public enum Emotion
{
    Neutral,
    Happy,
    Anger,
}

public enum MovingInStoryUI
{
    right,
    left,
    up,
    down,
}