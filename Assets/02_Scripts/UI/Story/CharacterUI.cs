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

    private Dictionary<int, Character> dataDict; // 캐릭터 id -> 객체

    private Dictionary<int, int> currentSlotMap = new(); // characterID → slotIndex
    
    private void Awake()
    {
        dataDict = new Dictionary<int, Character>();

        foreach (var data in characterDatas)
        {
            dataDict[data.characterID] = data;
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        // 혹시 남아있는 상태 초기화
        foreach (var slot in slots)
        {
            slot.ClearImmediate();
        }

        currentSlotMap.Clear();
    }
    
    public void SetupScene(List<CharacterInScene> newChars)
    {
        if (newChars == null)
        {
            // 유지
            return;
        }

        if (newChars.Count == 0)
        {
            // 명시적 초기화
            foreach (var slot in slots)
            {
                if(slot.isActiveAndEnabled)
                    slot.Disappear();
            }

            currentSlotMap.Clear();
            return;
        }
        
        HashSet<int> newIDs = new();

        // 1. 새로 등장 or 유지
        foreach (var info in newChars)
        {
            if (info.slotIndex < 0 || info.slotIndex >= slots.Count)
            {
                Debug.LogError($"slotIndex 오류: {info.slotIndex}"); 
                continue;
            }
            
            newIDs.Add(info.characterID);

            var slot = slots[info.slotIndex];

            // 이미 같은 슬롯에 있으면 유지
            if (currentSlotMap.TryGetValue(info.characterID, out int prevSlot))
            {
                if (prevSlot == info.slotIndex)
                    continue;
            }

            // 새로 등장
            if (dataDict.TryGetValue(info.characterID, out var data))
            {
                if (info.startVisible)
                {
                    // ⭐ 처음부터 보이는 캐릭터
                    slot.Appear(info.characterID, data.emotions[0].sprite);
                }
                else
                {
                    // ⭐ 아직 안 온 캐릭터 (숨김 상태)
                    slot.SetHidden(info.characterID, data.emotions[0].sprite);
                }
                currentSlotMap[info.characterID] = info.slotIndex;
            }
        }

        // 2. 사라진 캐릭터 처리
        var toRemove = new List<int>();

        foreach (var pair in currentSlotMap)
        {
            if (!newIDs.Contains(pair.Key))
            {
                int slotIndex = pair.Value;
                slots[slotIndex].Disappear();

                toRemove.Add(pair.Key);
            }
        }

        // 3. 딕셔너리 정리
        foreach (var id in toRemove)
        {
            currentSlotMap.Remove(id);
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
            slot.GetComponent<SpriteRenderer>().sprite = emo.sprite;
        }
    }
    
    public void SetUnknown(int characterID, bool isUnknown)
    {
        var slot = FindSlot(characterID);
        if (slot == null) return;

        if (isUnknown)
            slot.SetColor(new Color(0.1f, 0.1f, 0.1f, 1f));
        else
            slot.SetColor(Color.white);
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
                slot.SetColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
        }
    }

    public void PlayMotion(int characterID, MotionData motion, bool isUnknown)
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
                slot.Exit(Vector3.left, motion.value, motion.duration);
                break;

            case MotionType.ExitRight:
                slot.Exit(Vector3.right, motion.value, motion.duration);
                break;
            
            case MotionType.EnterLeft:
                slot.Enter(Vector3.left, motion.value, motion.duration, isUnknown);
                break;

            case MotionType.EnterRight:
                slot.Enter(Vector3.right, motion.value, motion.duration, isUnknown);
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
