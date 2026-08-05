using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
    [Header("�޴� ��ư")]
    [SerializeField] private Button btnQuests;
    [SerializeField] private Button btnSave;
    [SerializeField] private Button btnLoad;

    [Header("�˾� â")]
    [SerializeField] private RectTransform rectQuests;

    void Awake()
    {
        this.btnQuests.onClick.AddListener(OnClickQuests);
        btnSave.onClick.AddListener(OnClickSave);
        btnLoad.onClick.AddListener(OnClickLoad);
    }

    private void OnClickQuests()
    {
        if (this.rectQuests.gameObject.activeSelf)
        {
            UIAnimationManager.Instance.HidePaper(this.rectQuests, PaperDirection.TopRight,
                onComplete: () => UIManager.Instance.OpenQuestPopup(this.rectQuests.gameObject));
            return;
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
