using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button StartNewGameButton;
    [SerializeField] private Button ResumeGameButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button QuitButton;
    
    [SerializeField] private SaveLoadUI saveLoadUI;
    
    [SerializeField] private Setting setting;

    private void Start()
    {
        StartNewGameButton.onClick.AddListener(OnClickStartNewGameButton);
        ResumeGameButton.onClick.AddListener(OnClickResumeGameButton);
        SettingsButton.onClick.AddListener(OnClickSettingsButton);
        QuitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickStartNewGameButton()
    {  
        SceneManager.LoadScene("01_Scenes/03.Game");
        SaveManager.Instance.StartNewGame();
    }

    private void OnClickResumeGameButton()
    {
        SceneManager.LoadScene("01_Scenes/03.Game");
        SaveManager.Instance.LoadGame(0, SaveSlotType.Auto);
    }

    private void OnClickSettingsButton()
    {
        this.setting.Show();
    }

    private void OnClickQuitButton()
    {
        SaveManager.Instance.SaveAndQuit();
    }
    
    public void OnClickSave()
    {
        saveLoadUI.OpenSaveUI();
    }

    public void OnClickLoad()
    {
        saveLoadUI.OpenLoadUI();
    }
}
