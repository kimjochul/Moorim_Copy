using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [Header("슬롯들")]
    [SerializeField] private List<CharacterSlot> slots;

    [Header("캐릭터 데이터")]
    [SerializeField] private List<Character> characterDatas;

    private Dictionary<int, Character> dataDict;

    private void Awake()
    {
        dataDict = new Dictionary<int, Character>();

        foreach (var data in characterDatas)
        {
            dataDict[data.characterID] = data;
        }
    }
    
    public void SetupScene(List<CharacterInScene> characters)
    {
        // 1. 전부 비우기
        foreach (var slot in slots)
        {
            slot.Clear();
        }

        // 2. 순서대로 슬롯에 배치
        for (int i = 0; i < characters.Count && i < slots.Count; i++)
        {
            var info = characters[i];

            if (!dataDict.TryGetValue(info.characterID, out var data))
                continue;

            var defaultSprite = data.emotions[0].sprite;

            slots[i].SetCharacter(info.characterID, defaultSprite);
        }
    }
    
    public void SetExpression(int characterID, Emotion emotion)
    {
        var slot = FindSlot(characterID);
        if (slot == null) return;

        if (!dataDict.TryGetValue(characterID, out var data)) return;

        var emo = data.emotions.Find(e => e.emotion == emotion);
        if (emo != null)
        {
            slot.renderer.sprite = emo.sprite;
        }
    }

    public void Focus(int speakerID)
    {
        foreach (var slot in slots)
        {
            if (slot.CurrentCharacterID == speakerID)
            {
                slot.SetColor(Color.white);
            }
            else if (slot.CurrentCharacterID != -1)
            {
                slot.SetColor(new Color(0.5f, 0.5f, 0.5f, 0.7f));
            }
        }
    }

    public void PlayMotion(int characterID, MotionData motion)
    {
        var slot = FindSlot(characterID);
        if (slot == null || motion == null) return;

        switch (motion.type)
        {
            case MotionType.MoveLeft:
                slot.transform.position += Vector3.left * motion.value;
                break;

            case MotionType.MoveRight:
                slot.transform.position += Vector3.right * motion.value;
                break;

            case MotionType.Jump:
                StartCoroutine(Jump(slot.transform, motion));
                break;

            case MotionType.ExitLeft:
                slot.transform.position += Vector3.left * 5f;
                break;
            case MotionType.Shake: 
                StartCoroutine(slot.Shake(motion.duration, motion.value));
                break;
        }
    }
    
    private CharacterSlot FindSlot(int characterID)
    {
        return slots.Find(s => s.CurrentCharacterID == characterID);
    }

    
    IEnumerator Jump(Transform t, MotionData motion)
    {
        Vector3 start = t.position;
        Vector3 peak = start + Vector3.up * motion.value;

        float time = 0;

        while (time < motion.duration)
        {
            time += Time.deltaTime;
            float t01 = time / motion.duration;

            t.position = Vector3.Lerp(start, peak, Mathf.Sin(t01 * Mathf.PI));
            yield return null;
        }

        t.position = start;
    }
    
    public void ShowEmotionIcon(int characterID, Emotion emotion)
    {
        var slot = FindSlot(characterID);
        if (slot == null) return;

        if (!dataDict.TryGetValue(characterID, out var data)) return;

        var icon = data.emotionIcons.Find(e => e.emotion == emotion);

        if (icon != null)
        {
            slot.PlayEmotionIcon(icon.icon);
        }
    }
}
