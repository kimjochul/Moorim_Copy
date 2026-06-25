using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryReplaySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button slotButton; 
    
    [SerializeField] private Sprite slotSprite;
    [SerializeField] private Sprite selectedSprite;
    
    private Story story;
    private BriefStoryInfo briefStoryInfo;
    private StoryDetailPanel storyDetailPanel;
    private StoryReplayTab parentTab;

    private bool isSelected;
    private void OnEnable()
    {
        slotButton.onClick.AddListener(OnClick);
        isSelected = false;
    }

    private void OnDisable()
    {
        if(slotButton)
            slotButton.onClick.RemoveListener(OnClick);
    }
    
    public void Setup(Story story, BriefStoryInfo storyInfo, StoryDetailPanel detailPanel, StoryReplayTab parent)
    {
        this.story = story;
        briefStoryInfo = storyInfo;
        this.storyDetailPanel = detailPanel;
        parentTab = parent;
        
        titleText.text = story.storyName;
    }

    public void OnClick()
    {
        parentTab.SelectSlot(this);
        storyDetailPanel.SetStory(story);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        slotButton.image.sprite = isSelected ? selectedSprite : slotSprite;
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
