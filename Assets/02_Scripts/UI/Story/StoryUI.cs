using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private GameObject darkBackGround;
    [SerializeField] private DialogueLogUI dialogueLogUI;
    
    [SerializeField] private Image bgImage;
    [SerializeField] private Image illustrationA;
    [SerializeField] private Image illustrationB;
    
    [SerializeField] private Button dialogueButton;
    [SerializeField] private Button autoPlayButton;
    [SerializeField] private Image nextIndicator;
    
    [SerializeField] private float autoPlayDelay = 1.5f;
    private readonly WaitForSeconds typingWait = new(0.03f);
    
    [SerializeField] private ScreenEffectUI screenEffectUI;
    
    
    [Header("Dialogue Motion")]
    [SerializeField] private RectTransform dialogueRoot;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    
    [Header("Focus Mode")]
    [SerializeField] private float focusFade = 0.85f;
    [SerializeField] private float focusDuration = 0.25f;
    
    private Coroutine typingCoroutine;
    private Coroutine autoCoroutine;
    private Coroutine autoPlayCoroutine;
    
    private Coroutine illustrationCoroutine;
    private Coroutine illustrationSequenceCoroutine;

    private bool isTyping; // 현재 글자 나오고 있는 상황인지
    private bool isLogOpen;
    private bool isTransitioning; // 일러스트 전환 중인지
    private bool isCutSceneAuto; // 컷 씬용 대사 자동 재생 on인지
    private bool isAutoPlayEnabled; // 자동 재생 on off인지
    private bool isPointerBlocking; // 다른 버튼 눌렀을 때 대사 안 넘어가도록 bool 값 설정
    
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
    
    
    private Image currentIllust;
    private Image nextIllust;

    private CanvasGroup currentGroup;
    private CanvasGroup nextGroup;

    
    public void Init()
    {
        defaultContentFontSize = UIManager.Instance.GetTextScale();
        accentFontSize = UIManager.Instance.GetTextScale() * 1.3f;
        UIManager.Instance.OnFontSizeChanged += SetFontSize;
        
        currentIllust = illustrationA;
        nextIllust = illustrationB;
        currentGroup = currentIllust.GetComponent<CanvasGroup>();
        nextGroup = nextIllust.GetComponent<CanvasGroup>();
        currentGroup.alpha = 1f;
        nextGroup.alpha = 0f;

        currentIllust.gameObject.SetActive(false);
        nextIllust.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StoryManager.Instance.SetStoryUI(this);
        dialogueButton.onClick.AddListener(ToggleLogUI);
        autoPlayButton.onClick.AddListener(ToggleAutoPlay);
    }
    
    private void OnDisable()
    {
        if (dialogueButton != null)
            dialogueButton.onClick.RemoveListener(ToggleLogUI);

        if (autoPlayButton != null)
            autoPlayButton.onClick.RemoveListener(ToggleAutoPlay);

        if (UIManager.Instance != null)
            UIManager.Instance.OnFontSizeChanged -= SetFontSize;
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

        HandleKeyboardInput();
        HandleMouseInput();
        HandleScrollInput();
    }

    #region 대사 관련

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
        
        ApplyLineUI(line);
        
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
        
        SetDialogueUIVisible(!line.hideDialogueUI);
        currentFullText = line.line;

        typingCoroutine = StartCoroutine(TypeLine(line.line));
        if (line.autoNext)
        {
            isCutSceneAuto = true;
            
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

        int visibleCount = 0;
        int visibleCharCount = 0;

        while (visibleCount < text.Length)
        {
            if (text[visibleCount] == '<')
            {
                int closeIndex = text.IndexOf('>', visibleCount);

                if (closeIndex != -1)
                {
                    visibleCount = closeIndex + 1;
                }
            }
            else
            {
                char c = text[visibleCount];
                
                visibleCount++;
                visibleCharCount++;

                contentText.text = text[..visibleCount]; 
                
                if (!char.IsPunctuation(c) &&
                    !char.IsWhiteSpace(c) &&
                    visibleCharCount % 2 == 0)
                {
                    SoundManager.Instance.PlayTypeSound();
                }
                
                yield return typingWait;
                continue;
            }
            contentText.text = text[..visibleCount]; // 0 ~ visibleCount-1 까지
        }

        isTyping = false;
        ShowIndicator();
    }
    
    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

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

    private void SetFontSize(float size)
    {
        float targetSize = size;

        contentText.fontSize = targetSize;
    }
    
    #endregion
    
    #region 뒷 배경 관련

    public void ChangeBackground(Sprite bg, float duration)
    {
        if (isFirstBackground)
        {
            bgImage.sprite = bg;
            isFirstBackground = false;
            return;
        }
        
        if (bgCoroutine != null)
            StopCoroutine(bgCoroutine);

        bgCoroutine = StartCoroutine(FadeBackground(bg, duration));
    }
    
    public IEnumerator FadeBackground(Sprite newBg, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgImage.color = new Color(1,1,1,1 - time / duration);
            yield return null;
        }

        bgImage.sprite = newBg;

        time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgImage.color = new Color(1,1,1,time / duration);
            yield return null;
        }
    }
    
    private void ResetBackgroundState()
    {
        isFirstBackground = true;
        bgImage.sprite = null;
        bgImage.color = Color.white;
    }

    #endregion
    
    #region 일러스트 스토리 진행 화면

    public void ShowIllustration(Sprite sprite)
    {
        currentIllust.gameObject.SetActive(true);

        currentIllust.sprite = sprite;

        currentGroup.alpha = 1f;
    }

    public void ChangeIllustration(Sprite sprite, float duration)
    {
        if (illustrationCoroutine != null)
            StopCoroutine(illustrationCoroutine);

        illustrationCoroutine = StartCoroutine(CrossFadeIllustration(sprite, duration));
    }
    
    private IEnumerator CrossFadeIllustration(Sprite sprite, float duration)
    {
        isTransitioning = true;
        
        nextIllust.sprite = sprite;
        nextIllust.gameObject.SetActive(true);
        
        nextGroup.alpha = 0f;
        
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, time / duration);

            nextGroup.alpha = t;
            currentGroup.alpha = 1f - t;

            yield return null;
        }
        
        nextGroup.alpha = 1f;
        currentGroup.alpha = 0f;

        currentIllust.gameObject.SetActive(false);

        (currentIllust, nextIllust) = (nextIllust, currentIllust); // Swap
        (currentGroup, nextGroup) = (nextGroup, currentGroup); // Swap
        
        isTransitioning = false;
    }
    
    public void PlayIllustrationSequence(List<IllustrationFrame> frames)
    {
        if (illustrationSequenceCoroutine != null)
            StopCoroutine(illustrationSequenceCoroutine);

        illustrationSequenceCoroutine =
            StartCoroutine(PlayIllustrationSequenceRoutine(frames));
    }
    
    private IEnumerator PlayIllustrationSequenceRoutine(
        List<IllustrationFrame> frames)
    {
        if (frames == null || frames.Count == 0)
            yield break;

        isTransitioning = true;
        
        currentIllust.gameObject.SetActive(false);
        nextIllust.gameObject.SetActive(false);
        
        bool isFirstFrame = true;
        
        foreach (var frame in frames)
        {
            float duration = StoryManager.Instance.GetTransitionDuration(frame.transitionSpeed);

            if (isFirstFrame)
            {
                yield return StartCoroutine(screenEffectUI.FlashWhiteRoutine(1f, () => ShowIllustration(frame.sprite)));

                isFirstFrame = false;
            }
            else
            {
                // 두 번째 컷부터 transition
                yield return PlayIllustrationTransition(frame.sprite, duration, frame.transitionType);
            }
            
            yield return new WaitForSeconds(frame.duration);
        }
        
        isTransitioning = false;
    }
    
    private IEnumerator PlayIllustrationTransition(Sprite sprite, float duration, IllustrationTransitionType type)
    {
        switch (type)
        {
            case IllustrationTransitionType.Fade:
                yield return CrossFadeIllustration(sprite, duration);
                break;

            case IllustrationTransitionType.Flash:
                yield return FlashIllustration(sprite, duration);
                break;
        }
    }
    
    private IEnumerator FlashIllustration(Sprite sprite, float duration)
    {
        float flashIn = duration * 0.2f;
        float flashOut = duration * 0.8f;
        
        screenEffectUI.FlashWhite(duration);

        // 화면이 하얘질 때까지 기다림
        yield return new WaitForSeconds(flashIn);

        // 여기서 교체
        currentIllust.sprite = sprite;

        // flash 사라질 때까지 대기
        yield return new WaitForSeconds(flashOut);
    }
    
    public void HideIllustration()
    {
        currentIllust.gameObject.SetActive(false);
        nextIllust.gameObject.SetActive(false);
    }

    #endregion
    
    #region Indicator 관련
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
    #endregion

    #region 로그창 관련 함수
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
        if (isLogOpen)
        {
            CloseLogUI();
        }
        else
        {
            OpenLogUI();
        }
    }
    
    private void OpenLogUI()
    {
        isLogOpen = true;

        dialogueLogUI.Open();

        HideIndicator();
    }
    
    private void CloseLogUI()
    {
        isLogOpen = false;

        dialogueLogUI.Close();

        ShowIndicator();
    }
    #endregion

    #region 자동재생 관련 내용
    
    /// <summary>
    /// 컷 씬용 자동 재생 함수
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private IEnumerator AutoNextRoutine(StoryLine line)
    {
        while (isTyping)
            yield return null;

        float timer = 0f;

        while (timer < line.autoDelay)
        {
            // 로그 열려있으면 시간 멈춤
            if (!dialogueLogUI.gameObject.activeSelf && !isTransitioning)
            {
                timer += Time.deltaTime;
            }

            yield return null;
        }
        
        while (isTransitioning)
            yield return null;
        
        StoryManager.Instance.ShowNextLine();
        isCutSceneAuto = false;
    }
    
    private bool CurrentLineIsAuto()
    {
        return currentLine != null && currentLine.autoNext && currentLine.hideDialogueUI;
    }

    /// <summary>
    /// 수동으로 자동 재생 on할 때 사용하는 함수
    /// </summary>
    private void StartAutoPlay()
    {
        if (autoPlayCoroutine != null)
            StopCoroutine(autoPlayCoroutine);

        autoPlayCoroutine = StartCoroutine(AutoPlayRoutine());
    }

    /// <summary>
    /// 수동으로 자동 재생 off
    /// </summary>
    private void StopAutoPlay()
    {
        if (autoPlayCoroutine != null)
            StopCoroutine(autoPlayCoroutine);

        autoPlayCoroutine = null;
    }
    
    private IEnumerator AutoPlayRoutine()
    {
        while (isAutoPlayEnabled)
        {
            // 현재 타이핑 중이면 대기
            while (isTyping)
                yield return null;

            // 로그창 / 전환 중이면 대기
            while (!CanAdvance())
                yield return null;

            float timer = 0f;

            while (timer < autoPlayDelay)
            {
                // 중간에 막히면 시간 멈춤
                if (CanAdvance())
                {
                    timer += Time.deltaTime;
                }

                yield return null;
            }

            // 다시 한번 체크
            if (!CanAdvance())
                continue;

            // 현재 줄 자체 autoNext면 스킵
            if (CurrentLineIsAuto())
                continue;

            StoryManager.Instance.ShowNextLine();
        }
    }
    
    public void ToggleAutoPlay()
    {
        isAutoPlayEnabled = !isAutoPlayEnabled;

        if (isAutoPlayEnabled)
        {
            StartAutoPlay();
        }
        else
        {
            StopAutoPlay();
        }
    }
    #endregion

    #region 대사창에 효과

    public void FocusMode(bool enable)
    {
        dialogueRoot.DOKill();
        dialogueCanvasGroup.DOKill();

        if (enable)
        {
            dialogueCanvasGroup.DOFade(focusFade, focusDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            dialogueCanvasGroup.DOFade(1f, focusDuration).SetEase(Ease.OutCubic);
        }
    }

    #endregion
    
    #region 행동 가능 여부 확인 함수
    private bool CanAdvance()
    {
        return !IsBlocked();
    }

    private bool IsBlocked()
    {
        return
            isLogOpen ||
            isTransitioning ||
            isCutSceneAuto ||
            isPointerBlocking;
    }
    
    private bool CanScrollHistory()
    {
        return
            !isTyping &&
            !isTransitioning &&
            !isCutSceneAuto;
    }
    
    private bool CanSkip()
    {
        return
            !isTransitioning;
    }
    #endregion

    #region 입력 관련 함수
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLogOpen)
            {
                CloseLogUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (CanSkip())
            {
                StoryManager.Instance.SkipCurrentScene();
            }
        }
    }
    
    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        if (isPointerBlocking)
            return;
        
        if (isAutoPlayEnabled)
        {
            isAutoPlayEnabled = false;
            StopAutoPlay();
        }

        if (!CanAdvance())
            return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        StoryManager.Instance.ShowNextLine();
    }
    
    private void HandleScrollInput()
    {
        if (!CanScrollHistory())
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0)
        {
            ShowPrevious();
        }
        else if (scroll < 0)
        {
            ShowNext();
        }
    }
    
    public void SetPointerBlock(bool value)
    {
        isPointerBlocking = value;
    }
    
    public void StartReleasePointerBlock()
    {
        StartCoroutine(ReleasePointerBlockRoutine());
    }

    private IEnumerator ReleasePointerBlockRoutine()
    {
        yield return null;

        isPointerBlocking = false;
    }
    #endregion
}
