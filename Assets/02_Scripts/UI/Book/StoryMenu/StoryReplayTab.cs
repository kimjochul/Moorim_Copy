using UnityEngine;

public class StoryReplayTab : BaseTab
{
    [SerializeField] private StoryReplaySlot slotPrefab;
    [SerializeField] private Transform contentParent;
    
    [SerializeField] private BriefStoryInfo briefStoryInfo;
    [SerializeField] private StoryDetailPanel detailPanel;
    public void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (int storyID in StoryManager.Instance.StoryReplayManager.PlayedStoryHistory)
        {
            Story story = StoryManager.Instance.StoryReplayManager.GetStory(storyID);
            StoryReplaySlot slot = Instantiate(slotPrefab, contentParent);

            slot.Setup(story, briefStoryInfo, detailPanel);
        }
    }
    
    public override void SetupData()
    {
        
    }

    public override void ResetSetting()
    {
        Refresh();
    }
}
