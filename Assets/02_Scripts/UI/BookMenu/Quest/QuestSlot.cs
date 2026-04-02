using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class QuestSlot : MonoBehaviour
{
    private QuestData questData;
    [SerializeField] private Button btnSlot;
    [SerializeField] private TextMeshProUGUI txtQuestName;



    public void Init(QuestData data, Action<QuestData> onClickAction)
    {
        this.questData = data;

        this.txtQuestName.text = "";

        this.btnSlot.onClick.RemoveAllListeners();

        this.btnSlot.onClick.AddListener(() =>
        {
            if (this.questData != null)
            {
                onClickAction?.Invoke(this.questData);
            }
        });
    }
}
