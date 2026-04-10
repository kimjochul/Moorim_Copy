using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryTrigger
{
    public int storyId;

    public int requiredDay;
    public DayOfWeek? requiredDayOfWeek;

    public bool playOnce;
}

public class StoryManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private StorySO storySO;
    [SerializeField] private StoryUI storyUI;

    
    [SerializeField] private List<StoryTrigger> triggers;
    private HashSet<int> playedStories = new();
    
    private int currentIndex;
    private Story currentStory;
    
    private void Start()
    {
        dayManager.OnDayEnd += HandleDayEnd;
    }

    private void HandleDayEnd()
    {
        CheckStory(dayManager.DayData);
    }

    public void CheckStory(DayData dayData)
    {
        foreach (var trigger in triggers)
        {
            if (trigger.playOnce && playedStories.Contains(trigger.storyId))
                continue;

            if (dayData.day >= trigger.requiredDay)
            {
                PlayStory(trigger.storyId);
                playedStories.Add(trigger.storyId);
                break;
            }
        }
    }
    
    public void PlayStory(int id)
    {
        currentStory = storySO.stories.Find(s => s.id == id);
        currentIndex = 0;

        storyUI.Show();
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (currentIndex >= currentStory.story.Count)
        {
            EndStory();
            return;
        }

        var line = currentStory.story[currentIndex++];
        //storyUI.ShowLine(line.speaker, line.line);
    }

    private void EndStory()
    {
        storyUI.Hide();
    }
}
