using System;
using UnityEngine;

[RequireComponent(typeof(GuestBase))]
public class GuestInteractable : MonoBehaviour, IInteractable
{
    private GuestBase myBrain;

    public event Action<GuestBase> OnOpenServerInvenUI;

    void Awake()
    {
        myBrain = GetComponent<GuestBase>();
    }

    // 서빙 캐릭터만 상호작용가능
    public bool CanInteract(Constants.PlayerType playerType)
    {
        return playerType == Constants.PlayerType.Myungwol;
    }

    // 명월이가 손님을 클릭했을 때 맵의 어디로 걸어와야 하는가?
    public Vector3 GetInteractPosition()
    {
        // 현재 손님이 있는 위치(의자에 앉아있다면 의자 위치)로 걸어오게.
        // (만약 너무 손님과 겹친다면 transform.position 대신 myChair.sitPoint.position 등을 활용해도 좋을듯함)
        return transform.position;
    }

    // 명월이가 손님 근처에 도착해서 상호작용(F키 또는 클릭)을 완료했을 때 벌어질 일
    public void Interact()
    {
        if (myBrain.currentState == Constants.GuestState.WaitOrder)
            myBrain.TakeOrder();
        else if (myBrain.currentState == Constants.GuestState.WaitFood)
            OnOpenServerInvenUI?.Invoke(myBrain);
        else if (myBrain.currentState == Constants.GuestState.Exit)
        {
            // 도둑 손님인지 확인 (형변환)
            ThiefGuyGuest thief = myBrain as ThiefGuyGuest;
            
            if (thief != null)
                thief.CatchThief();
        }
        else
            Debug.Log($"상호작용 무시 (현재 상태: {myBrain.currentState})");
    }
}
