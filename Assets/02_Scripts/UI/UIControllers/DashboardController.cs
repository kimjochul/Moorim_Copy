using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
    [Header("�޴� ��ư")]
    [SerializeField] private Button btnOrders;
    [SerializeField] private Button btnQuests;
    [SerializeField] private Button btnSave;
    [SerializeField] private Button btnLoad;

    [Header("�˾� â")]
    [SerializeField] private RectTransform rectOrders;
    [SerializeField] private RectTransform rectQuests;

    void Awake()
    {
        this.btnOrders.onClick.AddListener(OnClickOrders);
        this.btnQuests.onClick.AddListener(OnClickQuests);
        btnSave.onClick.AddListener(OnClickSave);
        btnLoad.onClick.AddListener(OnClickLoad);
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

    private void OnClickSave()
    {
        UIManager.Instance.ActiveSaveUI(true);
    }

    private void OnClickLoad()
    {
        UIManager.Instance.ActiveLoadUI(true);
    }
}
