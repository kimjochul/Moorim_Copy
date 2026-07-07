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
    public int sortingOrder;
    
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
    
    [Header("Inner Thought")]
    [SerializeField] private GameObject dimObject;

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
            return;

        // 이전 상태 백업
        Dictionary<int, int> oldMap = new(currentSlotMap);
        currentSlotMap.Clear();

        HashSet<int> newIDs = new();
        foreach (var info in newChars)
        {
            if (!info.useSlotUI)
                continue;

            newIDs.Add(info.characterID);
        }

        // 제거될 슬롯 정리
        foreach (var slot in slots)
        {
            int currentID = slot.CurrentCharacterID;

            if (currentID == -1)
                continue;

            if (newIDs.Contains(currentID))
                continue;

            slot.StopAllCoroutines();
            slot.transform.DOKill();

            RectTransform rt = slot.SlotRoot;
            rt.DOKill();

            slot.ClearImmediate();
        }

        // 새 씬 세팅
        foreach (var info in newChars)
        {
            if (!info.useSlotUI)
                continue;

            if (info.slotIndex < 0 || info.slotIndex >= slots.Count)
                continue;

            if (!dataDict.TryGetValue(info.characterID, out var data))
                continue;

            var slot = slots[info.slotIndex];
            var preset = presets[info.slotIndex];
            RectTransform rt = slot.SlotRoot;

            // 슬롯 초기화
            slot.StopAllCoroutines();
            slot.transform.DOKill();
            rt.DOKill();

            Vector2 finalPos = preset.position;
            float finalScale = preset.scale;

            if (data.visual != null)
            {
                finalPos += data.visual.offset;
                finalScale *= data.visual.baseScale;
            }
            slot.transform.SetSiblingIndex(preset.sortingOrder);
            rt.anchoredPosition = finalPos;
            slot.SetBaseScale(finalScale);
            slot.SetFlip(info.startFlip);
            // 같은 캐릭터 유지 여부
            bool existedBefore = oldMap.TryGetValue(info.characterID, out int prevSlot);

            bool sameSlot = existedBefore && prevSlot == info.slotIndex;

            // 같은 슬롯 유지
            if (sameSlot)
            {
                currentSlotMap[info.characterID] = info.slotIndex;

                if (!info.startVisible)
                {
                    slot.SetHidden(data, data.characterImage);

                    continue;
                }

                // 이전엔 숨김이었는데 이번 씬에서 등장
                if (!slot.gameObject.activeSelf && info.startVisible)
                {
                    slot.SetCharacter(data, data.characterImage);
                }

                continue;
            }
            
            // 슬롯 변경된 같은 캐릭터
            if (existedBefore)
            {
                // 이전 슬롯 정리
                var oldSlot = slots[prevSlot];
                if (oldSlot != slot)
                {
                    oldSlot.StopAllCoroutines();
                    oldSlot.transform.DOKill();
                    RectTransform oldRt = oldSlot.GetComponent<RectTransform>();

                    oldRt.DOKill();
                    oldSlot.ClearImmediate();
                }
                
                if (info.startVisible)
                {
                    slot.SetCharacter(data, data.characterImage);
                }
                else
                {
                    slot.SetHidden(data, data.characterImage);
                }
                currentSlotMap[info.characterID] = info.slotIndex;
                Debug.Log($"SETUP character:{info.characterID} -> slot:{info.slotIndex}");
                Debug.Log($"REAL SLOT OBJECT : {slot.name}");
                continue;
            }

            // 완전 새 캐릭터
            if (info.startVisible)
            {
                slot.Appear(dataDict[info.characterID], data.characterImage, finalPos);
            }
            else
            {
                slot.SetHidden(dataDict[info.characterID], data.characterImage);
            }
            currentSlotMap[info.characterID] = info.slotIndex;
            
            Debug.Log($"SETUP character:{info.characterID} -> slot:{info.slotIndex}");
            Debug.Log($"REAL SLOT OBJECT : {slot.name}");
        }
        Debug.Log("-------------------------");
    }

    public void SetExpression(int characterID, ExpressionData expression)
    {
        CharacterSlot slot = FindSlot(characterID);

        if (slot == null)
            return;

        slot.SetExpression(expression);
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

    public void ApplyInnerThoughtFocus(int thinkerID, bool enable)
    {
        if (dimObject == null)
            return;

        dimObject.SetActive(enable);

        if (!enable)
        {
            dimObject.transform.SetAsLastSibling();
            return;
        }

        foreach (var slot in slots)
        {
            if (slot.CurrentCharacterID == -1)
                continue;

            int id = slot.CurrentCharacterID;

            if (!currentSlotMap.TryGetValue(id, out int slotIndex))
                continue;

            var preset = presets[slotIndex];

            if (enable)
            {
                if (id == thinkerID)
                {
                    // dim보다 위
                    slot.transform.SetAsLastSibling();
                }
                else
                {
                    // dim보다 아래
                    slot.transform.SetSiblingIndex(
                        Mathf.Min(preset.sortingOrder, dimObject.transform.GetSiblingIndex() - 1)
                    );
                }
            }
        }
    }
    
    public void SetFlip(int characterID, bool flip)
    {
        var slot = FindSlot(characterID);

        if (slot == null)
            return;

        slot.SetFlip(flip);
    }
    
    public IEnumerator PlayMotionRoutine(int characterID, MotionData motion, bool isUnknown)
    {
        var slot = FindSlot(characterID);
        
        if (slot == null || motion == null)
            yield break;

        if (motion.useFlip)
        {
            slot.SetFlip(motion.flip);
        }
        
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
                slot.transform.SetAsLastSibling();
                yield return slot.MoveRoutine(Vector2.left, motion.value, motion.duration);
                break;

            case MotionType.MoveRight:
                slot.transform.SetAsLastSibling();
                yield return slot.MoveRoutine(Vector2.right, motion.value, motion.duration);
                break;
            
            case MotionType.MoveToTarget:
            {
                if (motion.targetSlotIndex < 0 ||
                    motion.targetSlotIndex >= slots.Count)
                    yield break;
                
                slot.transform.SetAsLastSibling();
                var target = slots[motion.targetSlotIndex];

                Vector2 targetPos = target.SlotPos;
                targetPos.y = slot.SlotPos.y; // 현재 높이 유지
                Vector2 dir = GetDirection(slot, target);

                Vector2 destination = targetPos - dir * motion.stopDistance;
                
                yield return slot.MoveToPosition(destination, motion.duration);

                break;
            }
            
            case MotionType.ReturnSlot:
            {
                Vector2 originalPos = presets[slot.slotIndex].position;

                if (dataDict[characterID] != null && dataDict[characterID].visual != null)
                {
                    originalPos += dataDict[characterID].visual.offset;
                }
                yield return slot.MoveToPosition(originalPos, motion.duration); 

                slot.transform.SetSiblingIndex(preset.sortingOrder);
                break;
            }

            case MotionType.Jump:
                yield return slot.JumpRoutine(motion.value, motion.duration);
                break;
            
            case MotionType.ExitLeft:
                slot.transform.SetAsLastSibling();
                yield return slot.ExitRoutine(Vector2.left, motion.value, motion.duration);
                break;

            case MotionType.ExitRight:
                slot.transform.SetAsLastSibling();
                yield return slot.ExitRoutine(Vector2.right, motion.value, motion.duration);
                break;

            case MotionType.EnterLeft:
                slot.transform.SetAsLastSibling();
                yield return slot.EnterRoutine(finalPos, Vector2.left, motion.value, motion.duration, isUnknown);
                break;

            case MotionType.EnterRight:
                slot.transform.SetAsLastSibling();
                yield return slot.EnterRoutine(finalPos, Vector2.right, motion.value, motion.duration, isUnknown);
                break;

            case MotionType.Shake:
                yield return slot.Shake(motion.duration, motion.value);
                break;

            case MotionType.Emphasis:
                slot.transform.SetAsLastSibling();
                yield return slot.EmphasisRoutine();
                break;

            case MotionType.Nervous:
                slot.transform.SetAsLastSibling();
                yield return slot.NervousRoutine();
                break;
            
            case MotionType.Sit:
                yield return slot.SitRoutine(motion.value, motion.duration);
                break;

            case MotionType.Stand:
                slot.transform.SetAsLastSibling();
                yield return slot.StandRoutine(motion.duration);
                break;
            
            case MotionType.Attack:
            {
                slot.transform.SetAsLastSibling();
                var target = FindSlot(motion.targetSlotIndex);
                Vector2 dir = target != null ? GetDirection(slot, target) : Vector2.right;
                if (motion.value >= 50f)
                {
                    StoryManager.Instance.ScreenEffectUI.PlayImpact(motion.value, motion.duration, false);
                }
                yield return slot.PlayAttack(dir, motion.value, motion.duration * 0.5f);
                break;
            }
            
            case MotionType.EnterMove:
            {
                slot.transform.SetAsLastSibling();
                Vector2 targetPos = new Vector2(motion.targetValue, slot.SlotRoot.anchoredPosition.y);
                yield return slot.EnterMoveRoutine(motion.value, targetPos, motion.duration, isUnknown);
                break;
            }
        }
    }
    
    public IEnumerator PlayMotionRoutine(MotionCommand cmd, bool isUnknown)
    {
        var actor = FindSlot(cmd.characterID);

        if (actor == null)
            yield break;

        var motion = cmd.motion;

        if (motion.useFlip)
        {
            actor.SetFlip(motion.flip);
        }
        
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
                yield return PlayMotionRoutine(cmd.characterID, motion, isUnknown);
                break;
            }
        }
    }
    
    
    public IEnumerator PlayMotionsParallel(List<MotionCommand> motions, bool isUnknown)
    {
        if (motions == null || motions.Count == 0)
            yield break;

        int completeCount = 0;

        foreach (var cmd in motions)
        {
            StartCoroutine(ParallelMotionRoutine(cmd, isUnknown, () =>
            {
                completeCount++;
            }));
        }

        yield return new WaitUntil(() => completeCount >= motions.Count);
    }

    private IEnumerator ParallelMotionRoutine(MotionCommand cmd, bool isUnknown, Action onComplete)
    {
        yield return StartCoroutine(PlayMotionRoutine(cmd, isUnknown));
        onComplete?.Invoke();
    }
    
    private Vector2 GetDirection(CharacterSlot from, CharacterSlot to)
    {
        RectTransform a = from.GetComponent<RectTransform>();
        RectTransform b = to.GetComponent<RectTransform>();

        Vector2 result = (b.anchoredPosition - a.anchoredPosition);
        result.y = 0;
        
        return result.normalized;
    }
    
    public CharacterSlot FindSlot(int characterID)
    {
        if (!currentSlotMap.TryGetValue(characterID, out int slotIndex))
            return null;

        if (slotIndex < 0 || slotIndex >= slots.Count)
            return null;

        return slots[slotIndex];
    }
    
    public void ShowEmotionIcon(int characterID, Emotion emotion)
    {
        var slot = FindSlot(characterID);
        if (slot == null) return;

        if (!dataDict.TryGetValue(characterID, out var data)) return;

        var icon = data.emotionIcons.Find(e => e.emotion == emotion);

        if (icon != null)
        {
            slot.PlayEmotionIcon(emotion);
        }
    }
    
    public Character GetCharacterData(int characterID)
    {
        if (dataDict.TryGetValue(characterID, out var data))
            return data;

        return null;
    }
}
