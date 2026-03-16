using UnityEngine;

public interface IControllable
{
    void OnSelected();                  // 선택 되었을때
    void OnDeselected();                // 선택 해제되었을때
    void MoveTo(Vector3 destination);   // 목표 지점으로 이동
    void SetSpeed(float move, float angular, float accel);  //속도 조정
}
