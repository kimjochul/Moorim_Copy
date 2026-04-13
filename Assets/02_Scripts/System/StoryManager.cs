using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterName
{
    public int id;
    public string name;
}

public class StoryManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private StorySO storySO;

    [SerializeField] private List<StoryTrigger> triggers;

    [SerializeField] private RestaurantEconomy economy;
    
    [SerializeField] private StoryUI storyUI;
    [SerializeField] private CharacterUI characterUI;

    [SerializeField] private List<CharacterName> characterNames;
    private Dictionary<int, string> nameDict;
    
    /// <summary>
    /// flag 내용들을 저장데이터로 넣어놓으면 스토리 이어서
    /// </summary>
    private Story currentStory; // 스토리 뭉치 어느건지
    private int currentIndex; // 한 스토리 뭉치 내에서 대사 인덱스
    private StoryTrigger currentTrigger; // 현재 트리거


    private bool isPlaying;

    private HashSet<int> playedStories = new();
    private HashSet<string> flags = new();
    
    private void Awake()
    {
        storyUI.Init(this);

        nameDict = new Dictionary<int, string>();
        foreach (var c in characterNames)
        {
            nameDict[c.id] = c.name;
        }
    }
    
    private void Start()
    {
        dayManager.OnDayEnd += HandleDayEnd;
    }
    
    private void HandleDayEnd()
    {
        CheckTriggers();
    }

    private void CheckTriggers()
    {
        var sorted = triggers.OrderByDescending(t => t.priority);
        
        foreach (var trigger in sorted)
        {
            if (trigger.playOnce && playedStories.Contains(trigger.storyId))
                continue;

            if (!CheckCondition(trigger))
                continue;

            PlayStory(trigger.storyId);
            playedStories.Add(trigger.storyId);
            currentTrigger = trigger;
            return; // 한 번에 하나만 실행
        }
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
        
        if (!CheckRecipeCondition(t.recipeCondition))
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
    
    public void PlayStory(int id)
    {
        currentStory = storySO.stories.Find(s => s.id == id);

        if (currentStory == null)
        {
            Debug.LogError("스토리 없음");
            return;
        }

        currentIndex = 0;
        isPlaying = true;

        storyUI.Show();
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (!isPlaying) return;

        if (currentIndex >= currentStory.lines.Count)
        {
            EndStory();
            return;
        }

        var line = currentStory.lines[currentIndex++];
        
        // 캐릭터 처리
        characterUI.SetExpression(line.characterID, line.emotion);
        characterUI.Move(line.characterID, line.movingInStoryUI);

        // UI 출력
        storyUI.ShowLine(line);
    }

    private void EndStory(int storyId = -1)
    {
        if (!string.IsNullOrEmpty(currentTrigger.addFlagOnComplete))
        {
            flags.Add(currentTrigger.addFlagOnComplete);
        }
        
        isPlaying = false;
        storyUI.Hide();
    }
    
    public string GetCharacterName(int id)
    {
        if (nameDict.TryGetValue(id, out var name))
            return name;

        return "???";
    }
}
