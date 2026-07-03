using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestDetailPanel : MonoBehaviour
{
    [Header("기본 UI")]
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI description;

    [Header("조건 UI")]
    [SerializeField] private Transform conditionContent;
    [SerializeField] private GameObject conditionPrefab;

    [Header("보상 UI")]
    [SerializeField] private Transform rewardContent;
    [SerializeField] private QuestRewardUI rewardPrefab;
    
    [Header("버튼")]
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private NPCQuestUI parent;
    private Quest currentQuest;

    private void OnEnable()
    {
        if (currentQuest != null)
            SubscribeConditions(true);
    }

    private void OnDisable()
    {
        if (currentQuest != null)
            SubscribeConditions(false);
    }

    private void SubscribeConditions(bool subscribe)
    {
        foreach (var condition in currentQuest.Conditions)
        {
            if (subscribe)
                condition.OnConditionUpdated += RefreshAll;
            else
                condition.OnConditionUpdated -= RefreshAll;
        }
    }
    
    private void RefreshAll()
    {
        RefreshConditions();
        SetupButton();
    }
    
    public void Show(Quest quest, NPCQuestUI parent)
    {
        this.parent = parent;
        currentQuest = quest;

        questName.text = quest.QuestName;
        description.text = quest.Data.questDetail;

        RefreshConditions();   
        RefreshRewards();
        
        SetupButton();        

        gameObject.SetActive(true);
    }

    private void RefreshConditions()
    {
        // 기존 제거
        foreach (Transform child in conditionContent)
            Destroy(child.gameObject);

        // 조건 생성
        foreach (var condition in currentQuest.Conditions)
        {
            var obj = Instantiate(conditionPrefab, conditionContent);
            var ui = obj.GetComponent<QuestConditionUI>();

            ui.Init(condition, !currentQuest.IsAccepted);
        }
    }
    
    private void RefreshRewards()
    {
        foreach (Transform child in rewardContent)
            Destroy(child.gameObject);

        foreach (var reward in currentQuest.Data.GetRewardEntries())
        {
            var ui = Instantiate(rewardPrefab, rewardContent);
            ui.Init(reward);
        }
    }

    private void SetupButton()
    {
        actionButton.onClick.RemoveAllListeners();
        actionButton.interactable = true;

        if (!currentQuest.IsAccepted)
        {
            buttonText.text = "수락";
            actionButton.onClick.AddListener(parent.AcceptQuest);
        }
        else if (currentQuest.CanClear())
        {
            buttonText.text = "완료";
            actionButton.onClick.AddListener(parent.ClearQuest);
        }
        else
        {
            buttonText.text = "조건 미달";
            actionButton.interactable = false;
        }
    }

    public void Clear()
    {
        currentQuest = null;

        foreach (Transform child in conditionContent)
            Destroy(child.gameObject);
        
        foreach (Transform child in rewardContent)
            Destroy(child.gameObject);

        gameObject.SetActive(false);
    }
}