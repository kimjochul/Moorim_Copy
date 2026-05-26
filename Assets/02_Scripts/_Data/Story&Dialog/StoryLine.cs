using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryLine
{
    public int id;
    
    [Header("대화인지 나레이션인지")]
    public LineType lineType;
    
    [Header("일러스트 단일 그림 화면인 경우")]
    public Sprite illustrationOverride;
    
    [Header("추가 오버레이 일러스트")]
    public Sprite illustrationOverlay;
    
    [Header("일러스트 리셋 여부")]
    public bool resetIllustration;
    public bool resetOverlay;
    
    [Header("일러스트 다중 그림 시퀀스의 경우")]
    public List<IllustrationFrame> illustrationSequence;
    
    [Header("캐릭터 슬롯의 경우 캐릭터 현재 캐릭터에 대한 정보 및 행동 설정")]
    public int characterID;
    public bool isUnknown;
    public Emotion emotion;
    public MotionData motion;
    public List<MotionCommand> motions;
    
    [Header("스크린 이펙트 관련")]
    public ScreenEffectData screenEffect;
    
    
    [Header("대사 본문")]
    [TextArea(3, 10)]
    public string line;
    
    [Header("연출")]
    public bool useTypingSound = true;
    
    [Header("생각인지 발화인지")]
    public bool isInnerThought;
    
    [Header("감정 이모지 사용하는지")]
    public bool showEmotionIcon;
    
    [Header("강조 대사인지")]
    public bool useAccentFontSize;
    public bool useSmallFontSize;
    
    [Header("대사 창을 내릴 지(대사 없이 그림만 나올 때 사용 가능)")]
    public bool hideDialogueUI;
    
    [Header("대사 자동 진행")]
    public bool autoNext;
    public float autoDelay = 1.5f;
    
    [Header("로그 창에 추가 안하는 경우")]
    public bool hideFromLog;
    
    [Header("장면 씬 전환 속도")]
    public TransitionSpeed transitionSpeed;
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
    public int targetSlotIndex = -1;
    public float stopDistance = 100f;
}

[Serializable]
public class IllustrationFrame
{
    [Header("Visual")]
    public Sprite illustration;
    public Sprite background;
    
    [Header("Overlay")]
    public Sprite overlayIllustration;
    
    [Header("Timing")]
    public float duration = 1f;
    
    [Header("Transition")]
    public TransitionSpeed transitionSpeed;
    public IllustrationTransitionType transitionType;

    [Header("Option")] 
    public bool keepPreviousBG;
    public bool useFlashTransition = false;
}

[Serializable]
public class ScreenEffectData
{
    public ScreenEffectType effectType = ScreenEffectType.None;

    public float effectDuration = 0.3f;
    public float effectPower = 30f;
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
    MoveToTarget,
    ReturnSlot
}

public enum ScreenEffectType
{
    None,

    FadeBlack,
    FadeFromBlack,

    FlashWhite,
    Shake,
    FlashWithShake,

    ZoomIn,
    ZoomOut,

    VignetteDark,
    VignetteClear,
    
    FlashBlack,
    
    ShakeAndSound
}

public static class DialogueStyle
{
    public static string Angry(string text)
    {
        return $"<color=#FF5555><b>{text}</b></color>";
    }

    public static string Whisper(string text)
    {
        return $"<alpha=#88><i>{text}</i>";
    }
}

public enum TransitionSpeed
{
    Default,
    Fast,
    Slow,
    Dramatic,
    NoTerm
}

public enum IllustrationTransitionType { Fade, Flash, }