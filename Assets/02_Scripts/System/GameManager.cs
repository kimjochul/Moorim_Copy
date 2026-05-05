using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum GameFlowState
{
    None,
    Gameplay,
    Receipt,
    Story,
    NPC
}

public class GameManager : Singleton<GameManager>
{
    private Receipt receipt;
    private List<INPCSchedule> npcs = new List<INPCSchedule>();
    
    private ReceiptData? cachedResult;
    private GuestResultTracker guest;
    
    private bool isTransitioning = false;
    private bool pendingNext = false;
    
    public GameFlowState CurrentState { get; private set; }

    private IEnumerator Start()
    {
        yield return null;

        receipt = FindFirstObjectByType<Receipt>();
        npcs = DayManager.Instance.NPCs;
        
        BindEvents();
    }

    public void SetState(GameFlowState state)
    {
        if (CurrentState == state) return;

        CurrentState = state;
        Debug.Log($"[GameFlow] State Changed → {state}");
    }
    
    public void CompleteState(GameFlowState state)
    {
        if (CurrentState != state)
            return;

        Debug.Log($"CompleteState 요청: {state}, 현재: {CurrentState}");
        Next();
    }
    
    private void Next()
    {
        if (isTransitioning)
        {
            pendingNext = true;
            return;
        }

        isTransitioning = true;

        StartCoroutine(NextRoutine());
    }

    private IEnumerator NextRoutine()
    {
        yield return null; // 한 프레임 잠금
        
        switch (CurrentState)
        {
            case GameFlowState.Gameplay:
                EnterState(GameFlowState.Receipt);
                break;

            case GameFlowState.Receipt:
                EnterState(GameFlowState.Story);
                break;

            case GameFlowState.Story:
                EnterState(GameFlowState.NPC);
                break;

            case GameFlowState.NPC:
                EnterState(GameFlowState.Gameplay);
                break;
        }
        
        isTransitioning = false;
        
        if (pendingNext)
        {
            pendingNext = false;
            Next();
        }
    }

    private void EnterState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.Gameplay:
                DayManager.Instance.DayStart();
                break;

            case GameFlowState.Receipt:
                receipt.TurnOnReceipt(cachedResult);
                break;

            case GameFlowState.Story:
                StoryManager.Instance.HandleStoryStart();
                break;

            case GameFlowState.NPC:
                bool anyNPCActive = false;
                foreach (var npc in npcs)
                {
                    Debug.Log(npc.ToString());
                    if (npc.IsAvailableDay())
                    {
                        Debug.Log(npc.ToString());
                        npc.ViewActive();
                        anyNPCActive = true;
                    }
                }
                if (!anyNPCActive)
                {
                    CompleteState(GameFlowState.NPC);
                }
                break;
        }
    }
    
    private void BindEvents()
    {
        guest = FindFirstObjectByType<GuestResultTracker>();
        guest.OnDayResultReady += OnDayResultReady;
    }
    
    private void OnDayResultReady(GuestResultData daily, GuestResultData total)
    {
        cachedResult = receipt.HandleDayResult(daily, total);
    }
    
    public void ResetFlow()
    {
        isTransitioning = false;
        pendingNext = false;
    }
}
