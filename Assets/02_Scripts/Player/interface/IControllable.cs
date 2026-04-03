using UnityEngine;
using UnityEngine.Events;

public interface IControllable
{
    PlayerState CurrentState { get; }
    void ChangeState(PlayerState state);    //���� ����
    void OnSelected();                  // ���� �Ǿ�����
    void OnDeselected();                // ���� �����Ǿ�����
    void MoveTo(Vector3 destination, UnityAction action = null);   // ��ǥ �������� �̵�
    void SetSpeed(float move, float angular, float accel);  //�ӵ� ����
    void OnSlotClicked(Slot slot);
}
