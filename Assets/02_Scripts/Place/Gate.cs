using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using static Constants; 

public class Gate : MonoBehaviour, IInteractable 
{
    [Header("Door Settings")] 
    public GameObject doorHandle0;
    public GameObject doorHandle1;
    public float openDoorTime = 1f;
    
    private Tween doorTween;
    
    // 이벤트 등 외부 의존성 전부 제거됨
    // ✅ 기본값을 true로 변경했습니다. (열린 상태로 시작)
    public static bool isOpen { get; private set; } = true; 

    // IInteractable 인터페이스
    public InteractMode Mode => InteractMode.Immediate; 
    
    // 문이 열리거나 닫혔을 때 발생할 이벤트
    public static event Action<bool> OnGateStateChanged;

    public bool CanInteract(PlayerType playerType)
    {
        return true; 
    }

    public void Interact(PlayerController player)
    {
        if (doorTween != null && doorTween.IsActive() && doorTween.IsPlaying()) 
            return;

        // 이제 isOpen이 true이므로, 첫 클릭 시 바로 CloseDoor()가 실행됩니다.
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    public void OnInteractEnd(PlayerController player) { }

    public Vector3 GetInteractPosition()
    {
        return transform.position;
    }
    
    private void OpenDoor()
    {
        isOpen = true; 

        doorTween?.Kill();
        
        // 시퀀스를 사용하여 두 문이 동시에 움직이도록 설정
        Sequence s = DOTween.Sequence();
        s.Join(doorHandle0.transform.DOLocalRotate(new Vector3(0, 90, 0), openDoorTime).SetEase(Ease.OutQuad));
        s.Join(doorHandle1.transform.DOLocalRotate(new Vector3(0, -90, 0), openDoorTime).SetEase(Ease.OutQuad));
        
        s.OnComplete(() => 
        {
            OnGateStateChanged?.Invoke(isOpen);
        });
        
        doorTween = s;
    }

    private void CloseDoor()
    {
        isOpen = false; 
        
        doorTween?.Kill();
        
        // 닫힐 때는 둘 다 (0, 0, 0)으로 복귀
        Sequence s = DOTween.Sequence();
        s.Join(doorHandle0.transform.DOLocalRotate(Vector3.zero, openDoorTime).SetEase(Ease.OutQuad));
        s.Join(doorHandle1.transform.DOLocalRotate(Vector3.zero, openDoorTime).SetEase(Ease.OutQuad));
        
        s.OnComplete(() => 
        {
            OnGateStateChanged?.Invoke(isOpen);
        });
        
        doorTween = s;
    }
}