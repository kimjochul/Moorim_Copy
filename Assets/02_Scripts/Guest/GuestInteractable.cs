using System;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(GuestBase))]
public class GuestInteractable : MonoBehaviour, IInteractable
{
    private GuestBase myBrain;
    public GuestBase MyBrain => myBrain;
 
    public event Action<GuestBase> OnOpenServerInvenUI;
 
    void Awake()
    {
        myBrain = GetComponent<GuestBase>();
    }
 
    // IInteractable 구현 
 
    // 손님은 클릭해서 상호작용 (ClickToInteract)
    public InteractMode Mode => InteractMode.ClickToInteract;
 
    // 서빙 캐릭터만 상호작용 가능
    public bool CanInteract(PlayerType playerType)
    {
        return playerType == PlayerType.Myungwol;
    }
 
    // 명월이가 손님을 클릭했을 때 맵의 어디로 걸어와야 하는가?
    public Vector3 GetInteractPosition()
    {
        return transform.position;
    }
 
    // 명월이가 손님 근처에 도착해서 상호작용을 완료했을 때 벌어질 일
    public void Interact(PlayerController player)
    {
        if (myBrain.currentState == GuestState.WaitOrder)
            myBrain.TakeOrder();
        else if (myBrain.currentState == GuestState.WaitFood)
        {
            OnOpenServerInvenUI?.Invoke(myBrain);
        }
        else if (myBrain.currentState == GuestState.Exit)
        {
            ThiefGuyGuest thief = myBrain as ThiefGuyGuest;
            
            if (thief != null)
                thief.CatchThief();
        }
        else
        {
            Debug.Log($"상호작용 무시 (현재 상태: {myBrain.currentState})");
        }
    }
 
    public void OnInteractEnd(PlayerController player)
    {
        // ClickToInteract — 트리거 퇴장으로 호출되지 않음
    }
}
