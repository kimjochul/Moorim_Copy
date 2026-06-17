using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorySettingMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private Button logButton;
    [SerializeField] private Button autoButton;
    [SerializeField] private Button sceneSkipButton;
    [SerializeField] private Button storySkipButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private ScrollMenu scrollMenu;
    
    private StoryUI storyUI;
    private bool isOpen;

    public void Init(StoryUI ui)
    {
        storyUI = ui;

        toggleButton.onClick.AddListener(scrollMenu.Toggle);

        logButton.onClick.AddListener(storyUI.ToggleLogUI);
        autoButton.onClick.AddListener(storyUI.ToggleAutoPlay);

        sceneSkipButton.onClick.AddListener(() =>
        {
            StoryManager.Instance.SkipCurrentScene();
        });

        storySkipButton.onClick.AddListener(() =>
        {
            StoryManager.Instance.SkipCurrentStory();
        });
    }
}
