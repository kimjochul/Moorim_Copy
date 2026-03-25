using UnityEngine;
using UnityEngine.UI;

public class QuestAcceptBar : MonoBehaviour
{
    [SerializeField] private Button acceptButton;

    private Quest _quest;

    public void Init(Quest quest)
    {
        _quest = quest;

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(OnClickAccept);
    }

    private void OnClickAccept()
    {
        QuestManager.Instance.AcceptQuest(_quest);
        Destroy(gameObject);
    }
}
