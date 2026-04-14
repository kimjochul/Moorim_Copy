using UnityEngine;
using UnityEngine.UI;

public class PostManUI : MonoBehaviour
{
    [SerializeField] private GameObject questPanel;
    [SerializeField] private NPCTalkPanel talkPanel;

    [SerializeField] private Transform questContent;
    [SerializeField] private GameObject questSlotPrefab;
    
    [SerializeField] private Transform clearContent;
    [SerializeField] private GameObject clearSlotPrefab;
    
    private Inventory inventory;
    
    [SerializeField] private Button questButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Button exitButton;
    
    private bool isInitialized = false;
    
    [SerializeField] private string[] greetingDialogues;
    [SerializeField] private string[] idleDialogues; 
    
    private int lastDialogIndex = -1;

    public enum PostmanState
    {
        Quest,
        Talk,
    }
    private PostmanState currentState;
    private void SetState(PostmanState state)
    {
        currentState = state;
        UpdateDialogue();
    }
    
    private void OnEnable()
    {
        QuestManager.Instance.OnWaitingQuestChanged += RefreshAccept;
        QuestManager.Instance.OnActiveQuestChanged += RefreshClear;

        RefreshAccept();
        RefreshClear();
        
        OpenTalk();
    }
    
    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnWaitingQuestChanged -= RefreshAccept;
            QuestManager.Instance.OnActiveQuestChanged -= RefreshClear;
        }
    }

    public void Init(Postman postman)
    {
        if (isInitialized) return;
        isInitialized = true;   
        
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Fridge);
        
        questButton.onClick.AddListener(OpenQuest);
        talkButton.onClick.AddListener(OpenTalk);
        exitButton.onClick.AddListener(Close);
        
        currentState = PostmanState.Talk;
    }

    private void RefreshAccept()
    {
        var quests = QuestManager.Instance.GetWaitingQuests(QuestType.PostMan);

        // 기존 UI 삭제 (여기의 경우 acceptbar)
        foreach (Transform child in questContent)
            Destroy(child.gameObject);

        // 새로 생성
        foreach (var quest in quests)
        {
            var obj = Instantiate(questSlotPrefab, questContent);
            obj.GetComponent<QuestAcceptBar>().Init(quest);
        }
    }
    
    private void RefreshClear()
    {
        var quests = QuestManager.Instance.GetQuests(QuestType.PostMan);

        // 기존 UI 삭제 (여기의 경우 clearbar)
        foreach (Transform child in clearContent)
            Destroy(child.gameObject); // 만약 최적화 한다면 바꿔야 할 수도

        foreach (var quest in quests)
        {
            var obj = Instantiate(clearSlotPrefab, clearContent);
            obj.GetComponent<QuestClearBar>().Init(quest, inventory);
        }
    }
    
    public void OpenQuest()
    {
        talkPanel.gameObject.SetActive(true);
        questPanel.SetActive(true);
        
        SetState(PostmanState.Quest);
    }

    public void OpenTalk()
    {
        talkPanel.gameObject.SetActive(true);
        questPanel.SetActive(false);

        SetState(PostmanState.Talk);
    }
    
    private void UpdateDialogue()
    {
        string dialogue = "";

        switch (currentState)
        {
            case PostmanState.Quest:
                dialogue = "일 좀 부탁드려도 될까요? 보상은 해드릴게요";
                break;

            case PostmanState.Talk:
                dialogue = GetTalkDialogue();
                break;

            default:
                dialogue = "";
                break;
        }
        //Debug.Log($"대사 출력: {dialogue}");
        talkPanel.ChangeText(dialogue);
    }

    private string GetRandomDialogue(string[] dialogues)
    {
        if (dialogues == null || dialogues.Length == 0)
            return "";

        int index;

        do
        {
            index = Random.Range(0, dialogues.Length);
        }
        while (dialogues.Length > 1 && index == lastDialogIndex);

        lastDialogIndex = index;
        return dialogues[index];
    }
    
    private string GetTalkDialogue()
    {
        // 30% 확률로 인사, 70% 잡담
        if (Random.value < 0.3f)
        {
            return GetRandomDialogue(greetingDialogues);
        }
        else
        {
            return GetRandomDialogue(idleDialogues);
        }
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
        DayManager dayManager = FindFirstObjectByType<DayManager>();
        dayManager.DayStart();
    }
}
