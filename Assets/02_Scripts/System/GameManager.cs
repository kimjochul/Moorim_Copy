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

public class StateNode
{
    public GameFlowState state;
    public GameFlowState nextState;

    public Action onEnter;
    public Action onExit;

    public StateNode(GameFlowState state, GameFlowState nextState,
        Action onEnter = null,
        Action onExit = null)
    {
        this.state = state;
        this.nextState = nextState;
        this.onEnter = onEnter;
        this.onExit = onExit;
    }
}

public class GameManager : Singleton<GameManager>
{
    private Dictionary<GameFlowState, StateNode> stateTable;
    
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
        InitStateTable();
    }
    
    public void SetState(GameFlowState state)
    {
        if (CurrentState == state) return;

        CurrentState = state;
        Debug.Log($"[GameFlow] State Changed → {state}");
    }

    private void ChangeState(GameFlowState newState)
    {
        if (CurrentState == newState)
            return;

        Debug.Log($"[GameFlow] {CurrentState} → {newState}");

        // exit
        if (stateTable.ContainsKey(CurrentState))
        {
            stateTable[CurrentState].onExit?.Invoke();
        }

        CurrentState = newState;

        // enter
        stateTable[CurrentState].onEnter?.Invoke();
    }
    
    public void CompleteState(GameFlowState state)
    {
        if (isTransitioning)
        {
            StartCoroutine(WaitAndComplete(state));
            return;
        }

        if (CurrentState != state)
        {
            Debug.LogWarning($"잘못된 CompleteState 호출: {state}, 현재: {CurrentState}");
            return;
        }
        
        Debug.Log($"CompleteState 요청: {state}, 현재: {CurrentState}");
        //Next();
        StartCoroutine(TransitionRoutine());
    }
    
    private IEnumerator WaitAndComplete(GameFlowState state)
    {
        yield return new WaitUntil(() => !isTransitioning);
        CompleteState(state);
    }
    
    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        yield return null;

        var node = stateTable[CurrentState];
        ChangeState(node.nextState);

        isTransitioning = false;
    }
    
    private void InitStateTable()
    {
        stateTable = new Dictionary<GameFlowState, StateNode>()
        {
            {
                GameFlowState.Gameplay,
                new StateNode(
                    GameFlowState.Gameplay,
                    GameFlowState.Receipt,
                    onEnter: () =>
                    {
                        DayManager.Instance.DayStart();
                    }
                )
            },
            {
                GameFlowState.Receipt,
                new StateNode(
                    GameFlowState.Receipt,
                    GameFlowState.Story,
                    onEnter: () =>
                    {
                        receipt.TurnOnReceipt(cachedResult);
                    }
                )
            },
            {
                GameFlowState.Story,
                new StateNode(
                    GameFlowState.Story,
                    GameFlowState.NPC,
                    onEnter: () =>
                    {
                        StoryManager.Instance.HandleStoryStart();
                    }
                )
            },
            {
                GameFlowState.NPC,
                new StateNode(
                    GameFlowState.NPC,
                    GameFlowState.Gameplay,
                    onEnter: () =>
                    {
                        bool anyNPC = false;

                        foreach (var npc in npcs)
                        {
                            if (npc.IsAvailableDay())
                            {
                                npc.ViewActive();
                                anyNPC = true;
                            }
                        }

                        if (!anyNPC)
                        {
                            CompleteState(GameFlowState.NPC);
                        }
                    }
                )
            }
        };
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
