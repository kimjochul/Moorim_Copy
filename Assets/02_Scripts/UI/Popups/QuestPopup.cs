using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopup : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _questNameText;
    [SerializeField] private TextMeshProUGUI _questDescriptionText;
    [SerializeField] private Image _questImage;

    private bool isShown = false;
    public bool IsShown => isShown;
    
    void Awake()
    {
        _closeButton.onClick.AddListener(() => Destroy(gameObject));
    }

    void OnEnable()
    {
        isShown = true;
    }

    public void Init(Quest quest)
    {
        _questNameText.text = quest.QuestName;
        // _questDescriptionText.text = quest.Description;
        // _questImage.sprite = quest.;
    }
}
