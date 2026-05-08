using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryLine
{
    public int id;
    
    public LineType lineType;
    public Sprite illustrationOverride;
    
    public int characterID;
    public Emotion emotion;
    public MotionData motion;
    public List<MotionCommand> motions;
    
    public ScreenEffectType screenEffect;
    public float effectDuration = 0.3f;
    public float effectPower = 30f;
    
    [TextArea(3, 10)]
    public string line;
    
    public bool isInnerThought;
    
    public bool showEmotionIcon;
    public bool useAccentFontSize;

    public bool isUnknown;
    
    public bool hideDialogueUI;
    
    public bool autoNext;
    public float autoDelay = 1.5f;

    public bool hideFromLog;
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

[Serializable]
public class MotionCommand
{
    public int characterID;
    public int targetID;
    public MotionData motion;
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
    EnterLeft,
    EnterRight,
    Jump,
    Shake,
    Attack,   // 추가
    Hit,      // 추가
    Emphasis,    // 강조
    Nervous,  // 떨림
}

public enum ScreenEffectType
{
    None,

    FadeBlack,
    FadeFromBlack,

    FlashWhite,

    Shake,

    ZoomIn,
    ZoomOut,

    VignetteDark,
    VignetteClear
}