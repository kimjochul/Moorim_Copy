using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private List<Character> characters;
    private Dictionary<int, Character> characterDict;

    private void Awake()
    {
        characterDict = new Dictionary<int, Character>();

        foreach (var c in characters)
        {
            characterDict[c.characterID] = c;
        }
    }

    public void SetExpression(int characterID, Emotion emotion)
    {
        if (!characterDict.TryGetValue(characterID, out var character))
            return;

        var emo = character.emotions.Find(e => e.emotion == emotion);
        if (emo != null)
        {
            character.renderer.sprite = emo.sprite;
        }
    }

    public void Move(int characterId, MovingInStoryUI motion)
    {
        if (!characterDict.TryGetValue(characterId, out var character))
            return;

        Vector3 pos = character.transform.position;

        switch (motion)
        {
            case MovingInStoryUI.left:
                pos.x -= 1f;
                break;
            case MovingInStoryUI.right:
                pos.x += 1f;
                break;
            case MovingInStoryUI.up:
                pos.y += 1f;
                break;
            case MovingInStoryUI.down:
                pos.y -= 1f;
                break;
        }

        character.transform.position = pos;
    }
}
