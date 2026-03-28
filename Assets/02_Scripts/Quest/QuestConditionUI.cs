using TMPro;
using UnityEngine;

public class QuestConditionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject checkIcon; 

    private QuestCondition condition;
    private bool isPreview;
    
    public void Init(QuestCondition condition, bool isPreview)
    {
        this.condition = condition;
        this.isPreview = isPreview;
        
        if (isPreview)
            checkIcon.SetActive(false);

        Refresh();
    }

    private void OnEnable()
    {
        if (condition != null)
            condition.OnConditionUpdated += Refresh;
    }

    private void OnDisable()
    {
        if (condition != null)
            condition.OnConditionUpdated -= Refresh;
    }

    public void Refresh()
    {
        descriptionText.text = condition.GetDescription();
        progressText.text = condition.GetProgressText();

        if (isPreview)
            progressText.color = condition.IsComplete() ? Color.green : Color.white;
        
        checkIcon.SetActive(condition.IsComplete()); // ✔ 표시
    }
}
