using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class PostManUI : MonoBehaviour
{
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private NPCTalkPanel talkPanel;

    [SerializeField] private NPCQuestUI questUI;
    [SerializeField] private UpgradeUI upgradeUI;
    
    private Inventory inventory;
    
    [SerializeField] private Button questButton;
    [SerializeField] private Button talkButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button exitButton;
    
    private bool isInitialized = false;
    
    [SerializeField] private string[] greetingDialogues;
    [SerializeField] private string[] idleDialogues; 
    
    private int lastDialogIndex = -1;

    private Postman postman;
    
    public enum PostmanState
    {
        Quest,
        Talk,
        Upgrade,
    }
    private PostmanState currentState;
    private void SetState(PostmanState state)
    {
        currentState = state;
        UpdateDialogue();
    }
    
    private void OnEnable()
    {
        if (postman.IsAvailableDay())
            postman.DayManager.OnDayStart += ResetDialogue;
        
        OpenTalk();
    }

    private void Start()
    {
        OpenTalk();
    }
    
    private void OnDisable()
    {
        if (postman.DayManager != null)
            postman.DayManager.OnDayStart -= ResetDialogue;
    }

    public void Init(Postman postman)
    {
        if (isInitialized) return;
        isInitialized = true;   
        
        this.postman = postman;
        
        inventory = InventoryManager.Instance.GetInventory(InventoryType.Fridge);
        
        questButton.onClick.AddListener(OpenQuest);
        talkButton.onClick.AddListener(OpenTalk);
        upgradeButton.onClick.AddListener(OpenUpgrade);
        exitButton.onClick.AddListener(Close);
        
        currentState = PostmanState.Talk;
    }

    public void ResetDialogue()
    {
        lastDialogIndex = -1;
    }
    
    public void OpenQuest()
    {
        talkPanel.gameObject.SetActive(true);
        questPanel.SetActive(true);
        
        questUI.Init(QuestType.PostMan,NPCType.PostMan);
        
        SetState(PostmanState.Quest);
    }

    public void OpenTalk()
    {
        talkPanel.gameObject.SetActive(false);
        talkPanel.gameObject.SetActive(true);
        questPanel.SetActive(false);

        talkPanel.Clear();
        
        SetState(PostmanState.Talk);
    }
    
    public void OpenUpgrade()
    {
        talkPanel.gameObject.SetActive(true);

        questPanel.SetActive(false);
        upgradePanel.SetActive(true);

        upgradeUI.Refresh();

        SetState(PostmanState.Upgrade);
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

            case PostmanState.Upgrade:
                dialogue = "무림객잔 운영에 도움이 될 만한 물건들을 가져왔답니다.";
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
        GameManager.Instance.CompleteState(GameFlowState.NPC);
        //DayManager.Instance.DayStart();
    }
}
