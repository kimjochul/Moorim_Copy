using UnityEngine;
using UnityEngine.UI;

public class InventoryLengthRegulate : MonoBehaviour
{
    private GridLayoutGroup gridLayout;

    private void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
    }

    // 오브젝트가 활성화될 때(플레이어가 다가가서 켜질 때) 무조건 한 번 실행
    private void OnEnable()
    {
        UpdateConstraint();
    }

    private void OnTransformChildrenChanged()
    {
        // 오브젝트가 켜져 있는 상태에서만 갱신 (에러 방지)
        if (gameObject.activeInHierarchy) 
        {
            UpdateConstraint();
        }
    }

    private void UpdateConstraint()
    {
        if (gridLayout == null) 
            return;

        int actualSlotCount = 0;
        
        // 내 밑에 있는 모든 자식들을 하나씩 검사합니다.
        foreach (Transform child in transform)
        {
            // 자식의 이름에 "Slot"이 포함되어 있을 때만 개수를 셉니다.
            // (Instantiate로 생성되면 "Slot(Clone)"이 되므로 Contains로 잡힙니다)
            if (child.name.Contains("Slot")) 
            {
                actualSlotCount++;
            }
        }

        // 슬롯 개수에 맞춰 1~5 사이로 유동적 조절
        gridLayout.constraintCount = Mathf.Clamp(actualSlotCount, 1, 5);
    }
}