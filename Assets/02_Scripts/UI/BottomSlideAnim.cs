using UnityEngine;
using static Constants;

public class BottomSlideAnim : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("애니메이션 설정")]
    [SerializeField] private float hiddenPosY = -500f;   // 숨어있을 때의 Y 좌표 (인벤토리 창 높이만큼 마이너스)
    [SerializeField] private float shownPosY = 0f;       // 다 올라왔을 때의 Y 좌표

    private Coroutine currentAnim;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 시작할 때는 무조건 숨겨둔 상태로 시작
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, hiddenPosY);
    }

    public void OpenInventoryUI(Inventory inven)
    {
        if (inven.Type == InventoryType.Chef || inven.Type == InventoryType.Server)
            UIAnimationManager.Instance.ShowSlideUp(rectTransform, hiddenPosY, shownPosY);
    }

    public void CloseInventoryUI(Inventory inven)
    {
        if (inven.Type == InventoryType.Chef || inven.Type == InventoryType.Server)
            UIAnimationManager.Instance.HideSlideDown(rectTransform, hiddenPosY);
    }
}
