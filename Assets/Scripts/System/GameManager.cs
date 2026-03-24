using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 다른 매니저들을 관리하고 게임에서 플레이 흐름 제어하는 클래스
    [SerializeField] private DayManager dayManager;

    private void Start()
    {
        dayManager.DayStart();
    }

    public void StartDayButton()
    {
        dayManager.DayStart();
    }
}
