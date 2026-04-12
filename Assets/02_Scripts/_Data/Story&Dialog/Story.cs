using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Story
{
    public int id;
    public string storyName;

    [TextArea(3, 10)]
    public List<StoryScene> scenes;
}

[Serializable]
public class StoryScene
{
    public List<CharacterInScene> characters;
    public List<StoryLine> lines;
    public Sprite background; 
}

[Serializable]
public class CharacterInScene
{
    public int characterID;
    public Vector2 position;
}