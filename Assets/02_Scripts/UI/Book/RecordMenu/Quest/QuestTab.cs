using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;
using System.Collections.Generic;

[Serializable]
public class CategoryPair
{
    public QuestCategory category;
    public Toggle toggleTab;
}

public class QuestTab : BaseTab
{
    [SerializeField] private CategoryPair[] categoryTabs;

    private Dictionary<QuestCategory, List<Quest>> dicActiveQuests = new Dictionary<QuestCategory, List<Quest>>();
    private Dictionary<QuestCategory, List<Quest>> dicCompletedQuests = new Dictionary<QuestCategory, List<Quest>>();

    private QuestSlot currentSelectedSlot;

    [SerializeField] private QuestAccordionGroup inProgressGroup;
    [SerializeField] private QuestAccordionGroup completedGroup;

    [Header("왼쪽 페이지")]
    [SerializeField] private TextMeshProUGUI txtCategoryName;

    [Header("오른쪽 페이지")]
    [SerializeField] private TextMeshProUGUI txtQuestName;
    [SerializeField] private TextMeshProUGUI txtDetail;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform tsRewardParent;

    private QuestCategory currentCategory = QuestCategory.Main;

    private void Awake()
    {
        this.dicActiveQuests[QuestCategory.Main] = new List<Quest>();
        this.dicActiveQuests[QuestCategory.Sub] = new List<Quest>();
        this.dicActiveQuests[QuestCategory.Repeat] = new List<Quest>();

        this.dicCompletedQuests[QuestCategory.Main] = new List<Quest>();
        this.dicCompletedQuests[QuestCategory.Sub] = new List<Quest>();
        this.dicCompletedQuests[QuestCategory.Repeat] = new List<Quest>();
    }

    public override void SetupData()
    {
        if (this.currentSelectedSlot)
            this.currentSelectedSlot.SetSelect(false);

        QuestCache();
        CategoryTabInit();
        SetCategory(QuestCategory.Main);
        QuestListUpdate(this.currentCategory);
    }
    private void QuestListUpdate(QuestCategory targetCategory)
    {
        List<Quest> displayActiveQuests = this.dicActiveQuests[targetCategory];
        List<Quest> displayCompletedQuests = this.dicCompletedQuests[targetCategory];


        if (this.inProgressGroup != null)
            this.inProgressGroup.UpdateList(displayActiveQuests, ShowQuest);

        if (this.completedGroup != null)
            this.completedGroup.UpdateList(displayCompletedQuests, ShowQuest);

        ClearRightPanel();
    }

    public void ShowQuest(QuestSlot clickedSlot)
    {
        if (this.currentSelectedSlot != null)
            this.currentSelectedSlot.SetSelect(false);

        this.currentSelectedSlot = clickedSlot;
        this.currentSelectedSlot.SetSelect(true);

        QuestData data = clickedSlot.questData;

        // 오른쪽 페이지에 띄울 데이터
        this.txtQuestName.text = data.questName;
        this.txtDetail.text = data.questDetail;
        foreach(QuestCondition v in data.conditions)
            this.txtDescription.text = $"{v.GetDescription()}({v.GetProgressText()})";
        // 보상 추가해야함
    }

    public void CategoryTabInit()
    {
        for (int i = 0; i < this.categoryTabs.Length; i++)
        {
            int index = i;
            this.categoryTabs[i].toggleTab.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    this.currentCategory = this.categoryTabs[index].category;
                    QuestListUpdate(this.currentCategory);
                }
            });
        }
    }

    public void SetCategory(QuestCategory targetCategory)
    {
        foreach (var pair in this.categoryTabs)
        {
            if (pair.category == targetCategory)
            {
                pair.toggleTab.isOn = true;
                break;
            }
        }
    }


    private void ClearRightPanel()
    {
        if (this.currentSelectedSlot != null)
        {
            this.currentSelectedSlot.SetSelect(false);
            this.currentSelectedSlot = null;
        }

        this.txtQuestName.text = "선택된 퀘스트가 없습니다";
        this.txtDetail.text = "";
        this.txtDescription.text = "";
    }

    public void QuestCache()
    {
        foreach (var list in this.dicActiveQuests.Values)
            list.Clear();
        foreach (var list in this.dicCompletedQuests.Values)
            list.Clear();

        foreach (var q in QuestManager.Instance.ActiveQuests)
            AddQuestList(q, this.dicActiveQuests);

        foreach (var q in QuestManager.Instance.CompletedQuests)
            AddQuestList(q, this.dicCompletedQuests);
    }

    private void AddQuestList(Quest q, Dictionary<QuestCategory, List<Quest>> targetDic)
    {
        switch (q.QuestType)
        {
            case QuestType.MainStory:
                targetDic[QuestCategory.Main].Add(q);
                break;
            case QuestType.SubStory:
                targetDic[QuestCategory.Sub].Add(q);
                break;
            case QuestType.Merchant:
            case QuestType.PostMan:
                targetDic[QuestCategory.Repeat].Add(q);
                break;
        }
    }
}
