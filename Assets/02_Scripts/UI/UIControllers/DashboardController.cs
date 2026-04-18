using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
    [Header("메뉴 버튼")]
    [SerializeField] private Button btnOrders;
    [SerializeField] private Button btnQuests;

    [Header("팝업 창")]
    [SerializeField] private RectTransform rectOrders;
    [SerializeField] private RectTransform rectQuests;

    void Start()
    {
        this.btnOrders.onClick.AddListener(OnClickOrders);
        this.btnQuests.onClick.AddListener(OnClickQuests);
    }

    private void OnClickOrders()
    {
        if (this.rectOrders.gameObject.activeSelf)
        {
            UIAnimationManager.Instance.HidePaper(this.rectOrders, PaperDirection.TopRight,
                onComplete: () => UIManager.Instance.OpenOrdersPopup(this.rectOrders.gameObject));
            return;
        }

        if (this.rectQuests.gameObject.activeSelf)
        {
            UIAnimationManager.Instance.HidePaper(this.rectQuests, PaperDirection.TopRight,
                onComplete: () => UIManager.Instance.OpenQuestPopup(this.rectQuests.gameObject));
        }

        UIManager.Instance.OpenOrdersPopup(this.rectOrders.gameObject);

        UIAnimationManager.Instance.ShowPaper(this.rectOrders, PaperDirection.TopRight);
    }

    private void OnClickQuests()
    {
        if (this.rectQuests.gameObject.activeSelf)
        {
            UIAnimationManager.Instance.HidePaper(this.rectQuests, PaperDirection.TopRight,
                onComplete: () => UIManager.Instance.OpenQuestPopup(this.rectQuests.gameObject));
            return;
        }

        if (this.rectOrders.gameObject.activeSelf)
        {
            UIAnimationManager.Instance.HidePaper(this.rectOrders, PaperDirection.TopRight,
                onComplete: () => UIManager.Instance.OpenOrdersPopup(this.rectOrders.gameObject));
        }

        UIManager.Instance.OpenQuestPopup(this.rectQuests.gameObject);

        UIAnimationManager.Instance.ShowPaper(this.rectQuests, PaperDirection.TopRight);
    }
}
