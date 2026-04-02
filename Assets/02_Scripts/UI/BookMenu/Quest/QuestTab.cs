using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Constants;

[Serializable]
public class CategoryPair
{
    public QuestCategory category;
    public Toggle toggleTab;
}

public class QuestTab : BaseTab
{
    [SerializeField] private CategoryPair[] categoryTabs;

    // 왼쪽 페이지
    [SerializeField] private TextMeshProUGUI txtCategoryName;
    [SerializeField] private GameObject objQuestSlot;
    [SerializeField] private Transform tsInProgressParent; // 진행 중 부모
    [SerializeField] private Transform tsCompletedParent;  // 완료 부모
    // 오른쪽 페이지


    private QuestCategory currentCategory = QuestCategory.Main;
    protected override void OnEnable()
    {
        base.OnEnable();
        SetCategory(QuestCategory.Main);
    }
    void Start()
    {
        CategoryTabInit();
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
    public void CreateQuestSlot(QuestData data, Transform parent)
    {
        QuestSlot slot = Instantiate(this.objQuestSlot, parent).GetComponent<QuestSlot>();

        slot.Init(data, ShowQuest);
    }
    public void ShowQuest(QuestData data)
    {
        // 오른쪽 페이지에 띄울 데이터
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

                    RefreshUI();
                }
            });
        }
    }
    public override void RefreshUI()
    {
        // 카테고리 텍스트 변경
        switch (this.currentCategory)
        {
            case QuestCategory.Main:
                this.txtCategoryName.text = "메인 퀘스트";
                break;
            case QuestCategory.Sub:
                this.txtCategoryName.text = "서브 퀘스트";
                break;
            case QuestCategory.Repeat:
                this.txtCategoryName.text = "반복 퀘스트";
                break;
        }

    }
}
