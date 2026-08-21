using System.Collections.Generic;
using UnityEngine;

public class StoryReplayTab : BaseTab
{
    [SerializeField] private StoryReplaySlot slotPrefab;
    [SerializeField] private Transform contentParent;
    
    [SerializeField] private StoryDetailPanel detailPanel;
    
    private StoryReplaySlot currentSelectedSlot;

    public void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (int storyID in StoryManager.Instance.StoryReplayManager.PlayedStoryHistory)
        {
            Story story = StoryManager.Instance.StoryReplayManager.GetStory(storyID);
            StoryReplaySlot slot = Instantiate(slotPrefab, contentParent);
            
            slot.Setup(story, detailPanel, this);
        }
        
        detailPanel.gameObject.SetActive(false);
    }
    
    public void SelectSlot(StoryReplaySlot slot)
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(false);

        currentSelectedSlot = slot;

        if (currentSelectedSlot != null)
            currentSelectedSlot.SetSelected(true);
    }
    
    public override void SetupData()
    {
        
    }

    public override void ResetSetting()
    {
        Refresh();
    }
}
