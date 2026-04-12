using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryLine
{
    public int id;
    
    public int characterID;
    public Emotion emotion;
    public MotionData motion;
    
    public string line;
    
    public bool isInnerThought;
    
    public bool showEmotionIcon;
}

public enum Emotion
{
    Neutral,
    Happy,
    Anger,
}

[Serializable]
public class MotionData
{
    public MotionType type;
    public float duration = 0.5f;
    public float value = 1f; // 이동 거리 or 점프 높이
}

public enum MotionType
{
    None,
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    ExitLeft,
    ExitRight,
    Jump,
    Shake
}