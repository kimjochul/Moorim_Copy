using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryReplaySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button slotButton; 
    
    private Story story;
    private BriefStoryInfo briefStoryInfo;
    private StoryDetailPanel storyDetailPanel;

    private void OnEnable()
    {
        slotButton.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        if(slotButton)
            slotButton.onClick.RemoveListener(OnClick);
    }
    
    public void Setup(Story story, BriefStoryInfo storyInfo, StoryDetailPanel detailPanel)
    {
        this.story = story;
        briefStoryInfo = storyInfo;
        this.storyDetailPanel = detailPanel;
        
        titleText.text = story.storyName;
    }

    public void OnClick()
    {
        storyDetailPanel.SetStory(story);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        briefStoryInfo.SetInfo(story);
        briefStoryInfo.ShowInfo(transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        briefStoryInfo.HideInfo();
    }
}
