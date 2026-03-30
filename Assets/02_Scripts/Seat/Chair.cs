using UnityEngine;

public class Chair : MonoBehaviour
{
    private Table table;

    private bool isBuilt;
    private GuestBase currentGuest;

    private Transform sitPosition;

    public bool IsAvailable
    {
        get
        {
            // 의자가 설치되어있고, 테이블이 존재하며 설치되어있고, 현재 앉아있거나 찜한 손님이 없다면 => 빈자리 반환
            return isBuilt && table != null && table.isBuilt && currentGuest == null;
        }
    }
}
