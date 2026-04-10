using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryLine
{
    public int id;
    
    public int characterID;
    public Emotion emotion; 
    public MovingInStoryUI movingInStoryUI;
    
    public string line;
    
    public bool isInnerThought;
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