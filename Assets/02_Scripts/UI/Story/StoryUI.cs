using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private GameObject root;
    [SerializeField] private GameObject characterNameRoot;
    [SerializeField] private Image bgImage;
    [SerializeField] private GameObject darkBackGround;
    
    [SerializeField] private Image nextIndicator;
    
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine typingCoroutine;

    private bool isTyping;

    private StoryManager storyManager;

    private List<StoryLine> history = new();
    private int historyIndex = -1;
    
    private string currentFullText;
    
    private bool isFirstBackground = true;
    private Coroutine bgCoroutine;
    private Coroutine blinkCoroutine;

    public void Init(StoryManager manager)
    {
        storyManager = manager;
    }

    public void Show()
    {
        root.SetActive(true);
        ResetBackgroundState();
    }

    public void Hide()
    {
        root.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            storyManager.SkipCurrentScene();
            return;
        }

        // 클릭
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                storyManager.ShowNextLine();
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

        HideIndicator();
        
        if (line.lineType == LineType.Narration)
        {
            speakerText.text = "";
            contentText.alignment = TextAlignmentOptions.Center;
        }
        else
        {
            speakerText.text = storyManager.GetCharacterName(line.characterID);
            contentText.alignment = TextAlignmentOptions.TopLeft;
        }
        
        history.Add(line);
        historyIndex = history.Count - 1;
        
        ApplyLineUI(line);
        currentFullText = line.line;

        typingCoroutine = StartCoroutine(TypeLine(line.line));
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
            speakerText.text = storyManager.GetCharacterName(line.characterID);
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
            storyManager.ShowNextLine();
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
            contentText.color = Color.white;
            darkBackGround.SetActive(false);
        }
    }
    
    public void ChangeBackground(Sprite bg)
    {
        if (isFirstBackground)
        {
            bgCoroutine = StartCoroutine(FirstFadeIn(bg));
            isFirstBackground = false;
            return;
        }
        
        if (bgCoroutine != null)
            StopCoroutine(bgCoroutine);

        bgCoroutine = StartCoroutine(FadeBackground(bg));
    }
    
    IEnumerator FirstFadeIn(Sprite bg)
    {
        bgImage.sprite = bg;
        bgImage.color = new Color(1,1,1,0);

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgImage.color = new Color(1,1,1,time / fadeDuration);
            yield return null;
        }
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
}
