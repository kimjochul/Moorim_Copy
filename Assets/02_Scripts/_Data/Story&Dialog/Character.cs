using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Story/Character")]
public class Character : ScriptableObject
{
    public int characterID;
    public string characterName;

    public List<EmotionSprite> emotions;
    public List<EmotionIcon> emotionIcons;
    
    [Header("비주얼 보정")]
    public CharacterVisualSetting visual;
}

[Serializable]
public class EmotionSprite
{
    public Emotion emotion;
    public Sprite sprite;
}

[Serializable]
public class EmotionIcon
{
    public Emotion emotion;
    public Sprite icon;
}

[Serializable]
public class CharacterVisualSetting
{
    [Header("캐릭터 기본 크기 보정")]
    public float baseScale = 1f;

    [Header("캐릭터 위치 보정")]
    public Vector2 offset;
}