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
    
    [Header("씬 종료 연출")]
    public ScreenEffectData endEffect;
}

[Serializable]
public class CharacterInScene
{
    public int characterID;
    public int slotIndex;
    public bool startVisible;
    
    [Header("캐릭터 슬롯 UI에는 등장 안함")]
    public bool useSlotUI = true;
}

[Serializable]
public enum SceneVisualType
{
    CharacterSlots, // 기존 방식
    Illustration    // CG 컷씬 방식
}