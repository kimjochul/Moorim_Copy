using UnityEngine;

public class StoryReplayUI : MonoBehaviour
{
    [SerializeField] private StoryReplaySlot slotPrefab;
    [SerializeField] private Transform contentParent;

    public void Refresh()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (int storyID in StoryManager.Instance.StoryReplayManager.PlayedStoryHistory)
        {
            Story story = StoryManager.Instance.StoryReplayManager.GetStory(storyID);
            StoryReplaySlot slot = Instantiate(slotPrefab, contentParent);

            slot.Setup(story);
        }
    }
}
