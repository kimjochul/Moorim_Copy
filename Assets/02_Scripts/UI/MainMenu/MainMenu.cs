using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainMenu : MonoBehaviour
{
    [Header("메인 버튼들")]
    [SerializeField] private Button btnGameStart;
    [SerializeField] private Button btnGameContinue;
    [SerializeField] private Button btnGameSetting;
    void Awake()
    {
        EventInit();
    }
    private void EventInit()
    {
        this.btnGameStart.onClick.AddListener(OnClickGameStart);
        this.btnGameContinue.onClick.AddListener(OnClickGameContinue);
        this.btnGameSetting.onClick.AddListener(OnClickGameSetting);
    }
    private void OnClickGameStart()
    {
        MySceneManager.Instance.LoadSceneWithCallback(GAME);
    }

    private void OnClickGameContinue()
    {
        
    }

    private void OnClickGameSetting()
    {

    }
}
