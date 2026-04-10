using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public int characterID;
    public Transform transform;
    public SpriteRenderer renderer;

    public List<EmotionSprite> emotions;
}

[Serializable]
public class EmotionSprite
{
    public Emotion emotion;
    public Sprite sprite;
}