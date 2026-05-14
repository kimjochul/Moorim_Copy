using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using static Constants;

[Serializable]
public class CharacterName
{
    public int id;
    public string name;
}

[Serializable]
public class TriggerGroup
{
    // public int storytriggerID; // 만약 한 스토리를 접근하는 방향이 여러 방향(트리거)에서 접근 가능한 구조이면 id 필요
    public StoryTriggerSO so;
    public bool isActive;
}

public class StoryManager : Singleton<StoryManager>, IInitializable
{
    private DayManager dayManager;
    [SerializeField] private Receipt receipt;
    
    private RecipeUnlockManager recipeUnlockManager;
    private IngredientUnlockManager ingredientUnlockManager;
    
    [SerializeField] private StorySO storySO;

    [SerializeField] private List<TriggerGroup> triggerGroups;

    [SerializeField] private RestaurantEconomy economy;
    
    [Header("사운드")]
    [SerializeField] private AudioClip hitSE;
    
    private StoryUI storyUI;
    public StoryUI StoryUI => storyUI;
    private CharacterUI characterUI;
    private StoryFadePanel fadePanel;
    private ScreenEffectUI screenEffectUI;

    [SerializeField] private List<CharacterName> characterNames;
    private Dictionary<int, string> nameDict;

    [SerializeField] private float fadeDuration;
    
    private Tween transitionTween;
    private bool isSkipLocked;
    
    /// <summary>
    /// flag 내용들을 저장데이터로 넣어놓으면 스토리 이어서
    /// </summary>
    private Story currentStory; // 스토리 뭉치 어느건지
    private int currentIndex; // 한 스토리 뭉치 내에서 대사 인덱스
    private int currentSceneIndex;
    private StoryScene currentScene;
    private StoryTrigger currentTrigger; // 현재 트리거


    private bool isPlaying;
    private bool isTransitioning;

    private Queue<StoryTrigger> triggerQueue = new();
    
    private HashSet<int> playedStoryIds = new();
    private HashSet<string> flags = new();

    public void Init()
    {
        dayManager = DayManager.Instance;
        recipeUnlockManager = RecipeUnlockManager.Instance;
        ingredientUnlockManager = IngredientUnlockManager.Instance;
    }
    
    private void Start()
    {
        nameDict = new Dictionary<int, string>();
        foreach (var c in characterNames)
        {
            nameDict[c.id] = c.name;
        }
    }
    
    public void HandleStoryStart()
    {
        GameManager.Instance.SetState(GameFlowState.Story);
        
        CheckTriggers();
    }

    private void CheckTriggers()
    {
        var allTriggers = new List<StoryTrigger>();

        foreach (var group in triggerGroups)
        {
            if (!group.isActive) continue;

            allTriggers.AddRange(group.so.triggers);
        }

        var sorted = allTriggers.OrderByDescending(t => t.priority);

        foreach (var trigger in sorted)
        {
            if (trigger.playOnce && playedStoryIds.Contains(trigger.storyId))
                continue;

            if (!CheckCondition(trigger))
                continue;

            triggerQueue.Enqueue(trigger);
            if (trigger.playOnce)
                playedStoryIds.Add(trigger.storyId);
        }

        PlayNextQueuedStory();
    }
    
    private void PlayNextQueuedStory()
    {
        if (triggerQueue.Count == 0)
        {
            Debug.Log("스토리 없음 → NPC로 넘김");
            SoundManager.Instance.PlayRandomBGM();
            GameManager.Instance.CompleteState(GameFlowState.Story);
            return;
        }
        SoundManager.Instance.StopBGM();
        currentTrigger = triggerQueue.Dequeue();
        PlayStory(currentTrigger.storyId);
    }
    
    public void PlayStory(int id)
    {
        currentStory = storySO.stories.Find(s => s.id == id);
        storyUI.ClearLogs();
        
        storyUI.AddLogHeader(
            DayManager.Instance.DayData.day,
            currentStory.storyName
        );
        
        if (currentStory == null)
        {
            Debug.LogError("스토리 없음");
            return;
        }
        
        fadePanel.canvasGroup.gameObject.SetActive(true);
        fadePanel.canvasGroup.alpha = 1f;
        fadePanel.canvasGroup.blocksRaycasts = true;

        currentSceneIndex = 0;
        currentIndex = 0;
        isPlaying = true;

        storyUI.Show();
        
        if (HasAnyCharacter(currentStory))
            characterUI.Show();
        else
            characterUI.Hide();
        
        StartScene();
    }

    private void StartScene()
    {
        if (currentStory == null || currentSceneIndex >= currentStory.scenes.Count)
            return;

        DOVirtual.DelayedCall(0f, () =>
        {
            StartCoroutine(PlaySceneTransition(()=>SetupSceneInternal(false)));
        });
    }
    
    private IEnumerator SetupSceneInternal(bool useCrossFadeTransition) // 암전하는 fade를 사용 안하면 true
    {
        currentScene = currentStory.scenes[currentSceneIndex];
        currentIndex = 0;
            
        Coroutine illustRoutine = null;
        Coroutine bgRoutine = null;
        
        float duration = GetTransitionDuration(TransitionSpeed.Dramatic);
        
        if (currentScene.visualType == SceneVisualType.Illustration)
        {
            characterUI.Hide();
            
            if (useCrossFadeTransition)
            {
                illustRoutine = StartCoroutine(
                    storyUI.ChangeIllustrationRoutine(currentScene.illustration, duration)
                );
            }
            else
            {
                storyUI.ShowIllustration(currentScene.illustration);
            }
        }
        else
        {
            storyUI.HideIllustration();

            if (currentScene.characters == null || currentScene.characters.Count == 0)
                characterUI.Hide();
            else
                characterUI.Show();

            characterUI.SetupScene(currentScene.characters);
        }
            
        // Debug.Log($"{currentStory.id}");
        // foreach(var currentCharacter in currentScene.characters)
        //     Debug.Log(currentCharacter.characterID);

        if (useCrossFadeTransition)
        {
            bgRoutine = StartCoroutine(storyUI.ChangeBackgroundRoutine(currentScene.background, duration));
        }
        else
            storyUI.SetBackground(currentScene.background);
        
        if (illustRoutine != null)
            yield return illustRoutine;

        if (bgRoutine != null)
            yield return bgRoutine;
    }

    public void ShowNextLine()
    {
        if (!isPlaying || currentScene == null)
            return;
        
        if (isTransitioning) 
            return;
        
        // 🔥 씬 끝났을 때
        if (currentIndex >= currentScene.lines.Count)
        {
            StartCoroutine(HandleSceneEndRoutine());
            return;
        }
        
        var line = currentScene.lines[currentIndex++];
        StartCoroutine(ShowLineRoutine(line));
    }
    
    private IEnumerator ShowLineRoutine(StoryLine line)
    {
        
        float transitionDuration = GetTransitionDuration(line.transitionSpeed);
        
        storyUI.ShowLine(line);
        ApplyScreenEffect(line.screenEffect);
        ApplyCharacterFocusEffect(line);
        
        if (line.illustrationSequence != null && line.illustrationSequence.Count > 0)
            storyUI.PlayIllustrationSequence(line.illustrationSequence);
        else if (line.illustrationOverride != null)
            StartCoroutine(storyUI.ChangeIllustrationRoutine(line.illustrationOverride, transitionDuration));
        
        if (line.lineType == LineType.Dialogue)
        {
            storyUI.ShowCharacterName();
            
            characterUI.SetUnknown(line.characterID, line.isUnknown);
            characterUI.SetExpression(line.characterID, line.emotion);
            
            if (line.motions != null && line.motions.Count > 0)
            {
                foreach (var cmd in line.motions)
                {
                    yield return StartCoroutine(
                        characterUI.PlayMotionRoutine(cmd, line.isUnknown)
                    );
                }
            }
            else if (line.motion != null && line.motion.type != MotionType.None)
            {
                yield return StartCoroutine(
                    characterUI.PlayMotionRoutine(
                        line.characterID,
                        line.motion,
                        line.isUnknown
                    )
                );
            }
            
            characterUI.ShowEmotionIcon(line.characterID, line.emotion);
            
            if (line.isUnknown)
            {
                characterUI.Focus(-1); // 강조 없음
            }
            else
            {
                characterUI.Focus(line.characterID);
            }
        }
        else // ⭐ 나레이션
        {
            storyUI.HideCharacterName();
            characterUI.Focus(-1); // 아무도 강조 안함
        }
    }

    private IEnumerator HandleSceneEndRoutine()
    {
        yield return ApplyScreenEffectRoutine(currentScene.endEffect);
            
        SceneTransitionType transitionType = currentScene.transitionType;
            
        currentSceneIndex++;

        if (currentSceneIndex >= currentStory.scenes.Count)
        {
            EndStory();
            yield break;
        }

        switch (transitionType)
        {
            case SceneTransitionType.Fade:
                StartCoroutine(PlaySceneTransition(()=>SetupSceneInternal(false)));
                break;

            case SceneTransitionType.None:
                storyUI.ClearCurrentLine();
                yield return StartCoroutine(SetupSceneInternal(true));
                ShowNextLine();
                break;
        }
    }
    
    private void EndStory(int storyId = -1)
    {
        StartCoroutine(PlaySceneTransition(EndStoryRoutine, false));
    }

    private IEnumerator EndStoryRoutine()
    {
        if (currentTrigger != null && !string.IsNullOrEmpty(currentTrigger.addFlagOnComplete))
        {
            flags.Add(currentTrigger.addFlagOnComplete);
        }

        isPlaying = false;
        storyUI.Hide();
        characterUI.Hide();
            
        currentScene = null;

        if (currentStory.rewardQuest != null)
        {
            var quest = new Quest(currentStory.rewardQuest, InventoryManager.Instance.GetInventory(InventoryType.Fridge));
            QuestManager.Instance.AcceptQuest(quest);
        }
            
        if (currentTrigger != null && currentTrigger.unlockRecipeIds != null)
        {
            foreach (var recipeId in currentTrigger.unlockRecipeIds)
            {
                recipeUnlockManager.Unlock(recipeId);
            }
        }
        if (currentTrigger.unlockIngredientIds != null)
        {
            foreach (var id in currentTrigger.unlockIngredientIds)
            {
                ingredientUnlockManager.Unlock(id);
            }
        }
            
        PlayNextQueuedStory();
        yield break;
    }
    
    public void SkipCurrentScene()
    {
        if (!isPlaying || currentScene == null)
            return;
        
        if (isTransitioning || isSkipLocked)
            return;
        
        isSkipLocked = true;

        // 현재 씬 끝으로 이동
        currentIndex = currentScene.lines.Count;

        // 다음 ShowNextLine 호출 시 씬 종료 처리됨
        ShowNextLine();
        
        DOVirtual.DelayedCall(0.3f, () =>
        {
            isSkipLocked = false;
        });
    }
    
    public string GetCharacterName(int id)
    {
        if (nameDict.TryGetValue(id, out var name))
            return name;

        return "???";
    }
    
    private bool HasAnyCharacter(Story story)
    {
        foreach (var scene in story.scenes)
        {
            if (scene.characters != null && scene.characters.Count > 0)
                return true;
        }

        return false;
    }
    
    private bool CheckCondition(StoryTrigger t)
    {
        var dayData = dayManager.DayData;

        if (dayData.day < t.requiredDay)
            return false;

        if (t.requiredDayOfWeek.HasValue &&
            dayData.dayOfWeek != t.requiredDayOfWeek.Value)
            return false;

        if (economy.Money < t.requiredMoney)
            return false;

        if (economy.Fame < t.requiredFame)
            return false;

        if (economy.Debt < t.requiredDebt)
            return false;
        
        if (t.recipeCondition != null && !CheckRecipeCondition(t.recipeCondition))
            return false;

        if (!CheckFlagCondition(t))
            return false;
        
        return true;
    }
    
    private bool CheckRecipeCondition(RecipeCondition cond)
    {
        if (!cond.useCondition)
            return true;

        // 아무 레시피
        if (cond.anyRecipe)
        {
            return DataManager.Instance.HasAnyRecipeProficiency(cond.requiredProficiency);
        }

        // 특정 레시피
        var mastery = DataManager.Instance.GetRecipeMasteryData(cond.recipeId);

        if (mastery == null)
            return false;

        return mastery.proficiency >= cond.requiredProficiency;
    }
    
    private bool CheckFlagCondition(StoryTrigger t)
    {
        // 필요 플래그
        if (!string.IsNullOrEmpty(t.requiredFlag))
        {
            if (!flags.Contains(t.requiredFlag))
                return false;
        }

        // 막는 플래그
        if (!string.IsNullOrEmpty(t.blockedFlag))
        {
            if (flags.Contains(t.blockedFlag))
                return false;
        }

        return true;
    }
    
    public float GetTransitionDuration(TransitionSpeed speed)
    {
        return speed switch
        {
            TransitionSpeed.Fast => 0.5f,
            TransitionSpeed.Slow => 1.5f,
            TransitionSpeed.Dramatic => 2f,
            TransitionSpeed.Default => fadeDuration,
            TransitionSpeed.NoTerm => 0.05f,
            _ => fadeDuration
        };
    }
    
    private IEnumerator PlaySceneTransition(Func<IEnumerator> onMid, bool autoNextLine = true)
    {
        isTransitioning = true; // ⭐ 잠금
        
        fadePanel.canvasGroup.gameObject.SetActive(true);
        fadePanel.canvasGroup.blocksRaycasts = true;

        storyUI.ClearCurrentLine();
        yield return fadePanel.canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutBack).WaitForCompletion();
        
        if (onMid != null)
            yield return StartCoroutine(onMid());
        
        Canvas.ForceUpdateCanvases();

        yield return null;
        

        yield return fadePanel.canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        

        fadePanel.canvasGroup.blocksRaycasts = false;
        fadePanel.canvasGroup.gameObject.SetActive(false);
        
        isTransitioning = false;
        
        if (autoNextLine)
            ShowNextLine();
    }
    
    private void ApplyScreenEffect(ScreenEffectData effect)
    {
        if (effect == null)
            return;

        var effectUI = screenEffectUI;

        switch (effect.effectType)
        {
            case ScreenEffectType.None:
                break;

            case ScreenEffectType.FadeBlack:
                effectUI.FadeBlack(effect.effectDuration);
                break;

            case ScreenEffectType.FadeFromBlack:
                effectUI.FadeFromBlack(effect.effectDuration);
                break;

            case ScreenEffectType.FlashWhite:
                effectUI.FlashWhite(effect.effectDuration);
                break;

            case ScreenEffectType.Shake:
                effectUI.ShakeScreen(effect.effectPower, effect.effectDuration);
                break;

            case ScreenEffectType.VignetteDark:
                effectUI.SetVignette(0.45f, effect.effectDuration);
                break;

            case ScreenEffectType.VignetteClear:
                effectUI.SetVignette(0f, effect.effectDuration);
                break;

            case ScreenEffectType.FlashWithShake:
                effectUI.PlayImpact(effect.effectPower, effect.effectDuration, hitSE);
                break;
        }
    }
    
    private IEnumerator ApplyScreenEffectRoutine(ScreenEffectData effect)
    {
        if (effect == null)
            yield break;

        var effectUI = screenEffectUI;

        switch (effect.effectType)
        {
            case ScreenEffectType.None:
                yield break;

            case ScreenEffectType.FadeBlack:
                effectUI.FadeBlack(effect.effectDuration);
                break;

            case ScreenEffectType.FadeFromBlack:
                effectUI.FadeFromBlack(effect.effectDuration);
                break;

            case ScreenEffectType.FlashWhite:
                effectUI.FlashWhite(effect.effectDuration);
                break;

            case ScreenEffectType.Shake:
                effectUI.ShakeScreen(effect.effectPower, effect.effectDuration);
                break;

            case ScreenEffectType.VignetteDark:
                effectUI.SetVignette(0.45f, effect.effectDuration);
                break;

            case ScreenEffectType.VignetteClear:
                effectUI.SetVignette(0f, effect.effectDuration);
                break;

            case ScreenEffectType.FlashWithShake:
                effectUI.PlayImpact(effect.effectPower, effect.effectDuration, hitSE);
                break;
        }
        yield return new WaitForSeconds(effect.effectDuration);
    }
    
    private void ApplyCharacterFocusEffect(StoryLine line)
    {
        var slot = characterUI.FindSlot(line.characterID);

        if (slot == null)
            return;

        switch (line.screenEffect.effectType)
        {
            case ScreenEffectType.ZoomIn:
                screenEffectUI.ZoomTo(slot.SlotPos, 1.1f, line.screenEffect.effectDuration);
                storyUI.FocusMode(true);
                break;

            case ScreenEffectType.ZoomOut:
                screenEffectUI.ResetZoom(line.screenEffect.effectDuration);
                storyUI.FocusMode(false);
                break;
        }
    }
    
    public StorySaveData GetSaveData()
    {
        return new StorySaveData
        {
            playedStoryIDs = playedStoryIds.ToList(),
            flags = flags.ToList()
        };
    }
    
    public void LoadFromData(StorySaveData data)
    {
        playedStoryIds = new HashSet<int>(data.playedStoryIDs);
        flags = new HashSet<string>(data.flags);
    }
    
    public void ResetData()
    {
        playedStoryIds.Clear();
        flags.Clear();

        Debug.Log("Story Reset 완료");
    }
    
    public void SetStoryUI(StoryUI ui)
    {
        storyUI = ui;
        storyUI.Init();
    }
    
    public void SetCharacterUI(CharacterUI ui)
    {
        characterUI = ui;
    }
    
    public void SetReceipt(Receipt receipt)
    {
        this.receipt = receipt;
        //receipt.onButtonConfirm += HandleDayEnd; 
    }
    
    public void SetFadePanel(StoryFadePanel panel)
    {
        fadePanel = panel;
        
        fadePanel.canvasGroup.alpha = 0f;
        fadePanel.canvasGroup.gameObject.SetActive(false);
        fadePanel.canvasGroup.blocksRaycasts = false;
    }

    public void SetScreenEffectUI(ScreenEffectUI ui)
    {
        screenEffectUI = ui;
    }
}
