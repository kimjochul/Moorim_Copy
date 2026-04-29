using System;
using UnityEngine;

public enum GameFlowState
{
    None,
    Gameplay,
    Receipt,
    Story,
    Merchant
}

public class GameManager : MonoBehaviour
{
    public GameFlowState CurrentState { get; private set; }

    public void StartFlow(GameFlowState state)
    {
        CurrentState = state;
        EnterState(state);
    }

    public void Next()
    {
        switch (CurrentState)
        {
            case GameFlowState.Gameplay:
                StartFlow(GameFlowState.Receipt);
                break;
            case GameFlowState.Receipt:
                StartFlow(GameFlowState.Story);
                break;
            case GameFlowState.Story:
                StartFlow(GameFlowState.Merchant);
                break;
        }
    }

    private void EnterState(GameFlowState state)
    {
        // switch (state)
        // {
        //     case GameFlowState.Gameplay:
        //         DayManager.Instance.StartGameplay();
        //         break;
        //     case GameFlowState.Receipt:
        //         RecipeManager.Instance.Open();
        //         break;
        //     case GameFlowState.Story:
        //         StoryManager.Instance.StartStory();
        //         break;
        //     case GameFlowState.Merchant:
        //         MerchantManager.Instance.Open();
        //         break;
        // }
    }
}
