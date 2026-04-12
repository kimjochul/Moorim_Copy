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