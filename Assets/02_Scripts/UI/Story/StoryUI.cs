using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum LogType
{
    Dialogue,
    Header
}

[Serializable]
public class DialogueLog
{
    public LogType logType;
    
    public int day;
    public string title;
    
    public string speaker;
    public string content;

    public bool isNarration;
    public bool isInnerThought;
}

public class StoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private GameObject root;
    [SerializeField] private GameObject characterNameRoot;
    [SerializeField] private Image bgImage;
    [SerializeField] private GameObject darkBackGround;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private DialogueLogUI dialogueLogUI;
    
    [SerializeField] private Button dialogueButton;
    
    [SerializeField] private Image nextIndicator;
    
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine typingCoroutine;
    private Coroutine autoCoroutine;

    private bool isTyping;

    private List<StoryLine> history = new();
    private int historyIndex = -1;
    
    private List<DialogueLog> dialogueLogs = new();
    public IReadOnlyList<DialogueLog> DialogueLogs => dialogueLogs;
    
    private StoryLine currentLine;
    private string currentFullText;
    
    private bool isFirstBackground = true;
    private Coroutine bgCoroutine;
    private Coroutine blinkCoroutine;
    
    private float defaultContentFontSize;
    private float accentFontSize;
    
    private float ignoreClickTimer;
    private bool blockAdvanceInput;
    
    public void Init()
    {
        defaultContentFontSize = UIManager.Instance.GetTextScale();
        accentFontSize = UIManager.Instance.GetTextScale() * 1.3f;
        UIManager.Instance.OnFontSizeChanged += SetFontSize;
    }

    private void OnEnable()
    {
        StoryManager.Instance.SetStoryUI(this);
        dialogueButton.onClick.AddListener(ToggleLogUI);
    }
    
    public void Show()
    {
        root.SetActive(true);
        bgImage.gameObject.SetActive(true);
        ResetBackgroundState();
    }

    public void Hide()
    {
        root.SetActive(false);
        bgImage.gameObject.SetActive(false);
    }

    public void ShowIllustration(Sprite sprite)
    {
        illustrationImage.gameObject.SetActive(true);
        illustrationImage.sprite = sprite;
    }

    public void ChangeIllustration(Sprite sprite)
    {
        illustrationImage.sprite = sprite;
    }
    
    public void HideIllustration()
    {
        illustrationImage.gameObject.SetActive(false);
    }
    
    public void SetDialogueUIVisible(bool visible)
    {
        root.SetActive(visible);
    }
    
    public void ShowCharacterName()
    {
        characterNameRoot.SetActive(true);
    }

    public void HideCharacterName()
    {
        characterNameRoot.SetActive(false);
    }
    
    private void Update()
    {
        if (!root.activeSelf) return;
        
        if (blockAdvanceInput)
            return;
        
        if (ignoreClickTimer > 0f)
        {
            ignoreClickTimer -= Time.deltaTime;
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialogueLogUI.Close();
        }
        
        if (dialogueLogUI.gameObject.activeSelf)
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StoryManager.Instance.SkipCurrentScene();
            return;
        }

        // 클릭
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentLineIsAuto())
                return;
            
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                StoryManager.Instance.ShowNextLine();
            }
        }

        // 휠
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0)
        {
            ShowPrevious();
        }
        else if (scroll < 0)
        {
            ShowNext(); // ⭐ 추가
        }
    }

    public void ShowLine(StoryLine line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentLine = line;
        
        HideIndicator();
        
        defaultContentFontSize = UIManager.Instance.GetTextScale();
        accentFontSize = UIManager.Instance.GetTextScale() * 1.3f;
        if (line.useAccentFontSize)
            SetFontSize(accentFontSize);
        else
            SetFontSize(defaultContentFontSize);
        
        if (line.lineType == LineType.Narration)
        {
            speakerText.text = "";
            contentText.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            speakerText.text = line.isUnknown ? "???" : StoryManager.Instance.GetCharacterName(line.characterID);
            contentText.alignment = TextAlignmentOptions.TopLeft;
        }
        
        if (!line.hideFromLog)
        {
            history.Add(line);
            historyIndex = history.Count - 1;
            
            dialogueLogs.Add(new DialogueLog
            {
                logType = LogType.Dialogue,
                speaker = line.lineType == LineType.Narration ? "" : (line.isUnknown ? "???" : StoryManager.Instance.GetCharacterName(line.characterID)),
                content = line.line,
                isNarration = line.lineType == LineType.Narration,
                isInnerThought = line.isInnerThought
            });
        }
        
        ApplyLineUI(line);
        SetDialogueUIVisible(!line.hideDialogueUI);
        currentFullText = line.line;

        typingCoroutine = StartCoroutine(TypeLine(line.line));
        if (line.autoNext)
        {
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);

            autoCoroutine = StartCoroutine(AutoNextRoutine(line));
        }
    }

    private void ApplyLineUI(StoryLine line)
    {
        if (line.lineType == LineType.Narration)
        {
            speakerText.text = "";
            contentText.alignment = TextAlignmentOptions.Center;
            HideCharacterName();
        }
        else
        {
            speakerText.text = line.isUnknown ? "???" : StoryManager.Instance.GetCharacterName(line.characterID);
            contentText.alignment = TextAlignmentOptions.TopLeft;
            ShowCharacterName();
        }

        ApplyInnerThought(line.isInnerThought);
    }
    
    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        contentText.text = "";
        
        HideIndicator();

        foreach (char c in text)
        {
            contentText.text += c;

            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
        ShowIndicator();
    }
    
    private IEnumerator AutoNextRoutine(StoryLine line)
    {
        while (isTyping)
            yield return null;

        float timer = 0f;

        while (timer < line.autoDelay)
        {
            // 로그 열려있으면 시간 멈춤
            if (!dialogueLogUI.gameObject.activeSelf)
            {
                timer += Time.deltaTime;
            }

            yield return null;
        }

        StoryManager.Instance.ShowNextLine();
    }
    
    private bool CurrentLineIsAuto()
    {
        return currentLine != null && currentLine.autoNext && currentLine.hideDialogueUI;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        contentText.text = currentFullText; // ⭐ 핵심
        isTyping = false;
        ShowIndicator();
    }

    private void ShowPrevious()
    {
        if (isTyping) return;
        if (historyIndex <= 0) return;
        
        historyIndex--;
        var line = history[historyIndex];

        ApplyLineUI(line); 
        contentText.text = line.line;
    }
    
    private void ShowNext()
    {
        if (isTyping) return;
        
        if (historyIndex >= history.Count - 1)
        {
            // 최신이면 다음 라인 진행
            StoryManager.Instance.ShowNextLine();
            return;
        }

        historyIndex++;
        var line = history[historyIndex];

        ApplyLineUI(line);
        contentText.text = line.line;
    }

    private void ApplyInnerThought(bool isInner)
    {
        if (isInner)
        {
            contentText.color = Color.gray;
            darkBackGround.SetActive(true);
        }
        else
        {
            contentText.color = Color.black;
            darkBackGround.SetActive(false);
        }
    }
    
    public void ChangeBackground(Sprite bg)
    {
        if (isFirstBackground)
        {
            bgImage.sprite = bg;
            isFirstBackground = false;
            return;
        }
        
        if (bgCoroutine != null)
            StopCoroutine(bgCoroutine);

        bgCoroutine = StartCoroutine(FadeBackground(bg));
    }
    
    public IEnumerator FadeBackground(Sprite newBg)
    {
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgImage.color = new Color(1,1,1,1 - time / fadeDuration);
            yield return null;
        }

        bgImage.sprite = newBg;

        time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgImage.color = new Color(1,1,1,time / fadeDuration);
            yield return null;
        }
    }
    
    private void ShowIndicator()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        nextIndicator.enabled = true;
        
        blinkCoroutine = StartCoroutine(BlinkIndicator());
    }

    private void HideIndicator()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        nextIndicator.enabled = false;
    }
    
    private IEnumerator BlinkIndicator()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime * 1.5f;
            float alpha = Mathf.PingPong(time, 1f);

            var color = nextIndicator.color;
            color.a = alpha;
            nextIndicator.color = color;

            yield return null;
        }
    }
    
    public void ResetBackgroundState()
    {
        isFirstBackground = true;
        bgImage.sprite = null;
        bgImage.color = Color.white;
    }
    
    public void SetFontSize(float size)
    {
        float targetSize = size;

        contentText.fontSize = targetSize;
    }
    
    public void AddLogHeader(int day, string storyTitle)
    {
        dialogueLogs.Add(new DialogueLog
        {
            logType = LogType.Header,
            day = day,
            title = storyTitle,
        });
    }
    
    public void ClearLogs()
    {
        dialogueLogs.Clear();

        history.Clear();
        historyIndex = -1;
    }
    
    public void ToggleLogUI()
    {
        ignoreClickTimer = 0.15f;

        if (dialogueLogUI.gameObject.activeSelf)
        {
            dialogueLogUI.Close();
            ShowIndicator();
        }
        else
        {
            dialogueLogUI.Open();
            HideIndicator();
        }
    }
    
    public void OnLogButtonDown()
    {
        blockAdvanceInput = true;
    }
    public void OnLogButtonUp()
    {
        StartCoroutine(ReleaseInputBlock());
    }
    private IEnumerator ReleaseInputBlock()
    {
        yield return null;
        blockAdvanceInput = false;
    }
}
