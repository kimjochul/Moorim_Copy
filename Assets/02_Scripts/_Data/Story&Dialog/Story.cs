using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryScenes", menuName = "Story/StoryScenes")]
public class Story:ScriptableObject
{
    public int id;
    public string storyName;
    
    public List<StoryScene> scenes;
    
    public QuestData rewardQuest;
}

[Serializable]
public class StoryScene
{
    public SceneVisualType visualType;
    public List<CharacterInScene> characters;
    public List<StoryLine> lines;
    public Sprite background; 
    public Sprite illustration;
}

[Serializable]
public class CharacterInScene
{
    public int characterID;
    public int slotIndex;
    public bool startVisible;
}

[Serializable]
public enum SceneVisualType
{
    CharacterSlots, // 기존 방식
    Illustration    // CG 컷씬 방식
}