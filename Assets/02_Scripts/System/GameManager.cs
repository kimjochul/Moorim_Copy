using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 다른 매니저들을 관리하고 게임에서 플레이 흐름 제어하는 클래스
    private DayManager dayManager;
    private SaveManager saveManager;
    
    private void Awake()
    {
        saveManager = SaveManager.Instance;
        dayManager = DayManager.Instance;
    }

    private void Start()
    {
        dayManager.InitForGameScene();
    }
    
    private void OnApplicationQuit()
    {
        SaveManager.Instance.SaveGame();
    }
}
