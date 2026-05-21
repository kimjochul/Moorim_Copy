using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class SlotPreset
{
    public Vector2 position;
    public float scale;
    
    [Header("연출용")]
    public float focusScale;   // 강조 시 크기
}

public class CharacterUI : MonoBehaviour
{
    [Header("슬롯들")]
    [SerializeField] private List<CharacterSlot> slots;
    [SerializeField] private List<SlotPreset> presets;

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
    
    private void OnEnable()
    {
        StoryManager.Instance.SetCharacterUI(this);
    }
    
    public void Show()
    {
        
    }

    public void Hide()
    {
        // 혹시 남아있는 상태 초기화
        foreach (var slot in slots)
        {
            slot.ClearImmediate();
            slot.gameObject.SetActive(false);
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
                    slot.Disappear(slot.SlotPos);
            }

            currentSlotMap.Clear();
            return;
        }
        
        HashSet<int> newIDs = new();

        // 1. 새로 등장 or 유지
        foreach (var info in newChars)
        {
            if (!info.useSlotUI)
                continue;
            
            if (info.slotIndex < 0 || info.slotIndex >= slots.Count)
            {
                Debug.LogError($"slotIndex 오류: {info.slotIndex}"); 
                continue;
            }
            
            newIDs.Add(info.characterID);

            var slot = slots[info.slotIndex];
            var preset = presets[info.slotIndex];

            RectTransform rt = slot.GetComponent<RectTransform>();
            
            
            Vector2 finalPos = preset.position;
            float finalScale = preset.scale;

            if (dataDict[info.characterID] != null && dataDict[info.characterID].visual != null)
            {
                finalPos += dataDict[info.characterID].visual.offset;
                finalScale *= dataDict[info.characterID].visual.baseScale;
            }

            rt.anchoredPosition = finalPos;
            slot.SetBaseScale(finalScale);

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
                    slot.Appear(info.characterID, data.emotions[0].sprite, preset.position);
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
                slots[slotIndex].Disappear(slots[slotIndex].SlotPos);

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

        if (slot.CurrentEmotion == emotion)
            return;
        
        if (!dataDict.TryGetValue(characterID, out var data)) return;

        var emo = data.emotions.Find(e => e.emotion == emotion);
        if (emo != null)
        {
            slot.SetEmotion(emotion);
            slot.ChangeExpression(emo.sprite);
        }
    }
    
    public void SetUnknown(int characterID, bool isUnknown)
    {
        var slot = FindSlot(characterID);
        if (slot == null) return;

        slot.SetUnknown(isUnknown);
    }

    public void Focus(int speakerID)
    {
        foreach (var slot in slots)
        {
            if (slot.IsPlayingMotion)
                continue;
            
            int id = slot.CurrentCharacterID;
            if (id == -1) continue;
            
            int slotIndex = currentSlotMap[id];
            var preset = presets[slotIndex];
            
            RectTransform rt = slot.GetComponent<RectTransform>();
            
            if (id == speakerID)
            {
                slot.SetFocusScale(preset.focusScale, 0.2f);
                slot.SetFocused(true);
            }
            else
            {
                slot.SetFocusScale(preset.scale, 0.2f);
                slot.SetFocused(false);
            }
        }
    }

    public IEnumerator PlayMotionRoutine(int characterID, MotionData motion, bool isUnknown)
    {
        var slot = FindSlot(characterID);

        if (slot == null || motion == null)
            yield break;

        var preset = presets[slot.slotIndex];

        Vector2 finalPos = preset.position;
        if (dataDict[characterID] != null && dataDict[characterID].visual != null)
        {
            finalPos += dataDict[characterID].visual.offset;
        }
        
        Debug.Log($"{slot.slotIndex}, {slot.SlotPos} , {finalPos}");
        
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
                yield return slot.ExitRoutine(Vector2.left, motion.value, motion.duration);
                break;

            case MotionType.ExitRight:
                yield return slot.ExitRoutine(Vector2.right, motion.value, motion.duration);
                break;

            case MotionType.EnterLeft:
                yield return slot.EnterRoutine(finalPos, Vector2.left, motion.value, motion.duration, isUnknown);
                break;

            case MotionType.EnterRight:
                yield return slot.EnterRoutine(finalPos, Vector2.right, motion.value, motion.duration, isUnknown);
                break;

            case MotionType.Shake:
                yield return slot.Shake(motion.duration, motion.value);
                break;

            case MotionType.Emphasis:
                yield return slot.EmphasisRoutine();
                break;

            case MotionType.Nervous:
                yield return slot.NervousRoutine();
                break;
        }
    }
    
    public IEnumerator PlayMotionRoutine(MotionCommand cmd, bool isUnknown)
    {
        var actor = FindSlot(cmd.characterID);

        if (actor == null)
            yield break;

        var motion = cmd.motion;

        switch (motion.type)
        {
            case MotionType.Attack:
            {
                var target = FindSlot(cmd.targetID);

                Vector2 dir = target != null ? GetDirection(actor, target) : Vector2.right;

                // 공격 먼저
                yield return actor.PlayAttack(dir, motion.value, motion.duration * 0.5f);

                // 피격
                if (target != null)
                {
                    yield return target.PlayHit(-dir, motion.value * 0.5f, motion.duration * 0.5f);
                }

                break;
            }

            case MotionType.Hit:
            {
                var target = FindSlot(cmd.targetID);

                Vector2 dir = target != null ? GetDirection(target, actor) : Vector2.left;

                yield return actor.PlayHit(dir, motion.value, motion.duration);

                break;
            }

            default:
            {
                yield return PlayMotionRoutine(
                    cmd.characterID,
                    motion,
                    isUnknown);

                break;
            }
        }
    }
    
    private Vector2 GetDirection(CharacterSlot from, CharacterSlot to)
    {
        RectTransform a = from.GetComponent<RectTransform>();
        RectTransform b = to.GetComponent<RectTransform>();

        return (b.anchoredPosition - a.anchoredPosition).normalized;
    }
    
    public CharacterSlot FindSlot(int characterID)
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
