using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryReplaySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private Button slotButton; 
    
    private Story story;

    private void OnEnable()
    {
        slotButton.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        if(slotButton)
            slotButton.onClick.RemoveListener(OnClick);
    }

    public void Setup(Story story)
    {
        this.story = story;

        titleText.text = story.storyName;
        description.text = story.description;
    }

    public void OnClick()
    {
        string msg = $"{story.storyName}을 다시 보시겠습니까?";
        UIManager.Instance.CreateOkPopup(msg, () =>
        {
            StoryManager.Instance.StoryReplayManager.PlayStory(story.id);
        }, () => {}, true);
    }
}
