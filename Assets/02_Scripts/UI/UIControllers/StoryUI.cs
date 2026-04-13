using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private GameObject root;
    [SerializeField] private GameObject darkBackGround;

    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool isSkipping;

    private StoryManager storyManager;

    private List<string> history = new();
    private int historyIndex = -1;
    
    private string currentFullText;

    public void Init(StoryManager manager)
    {
        storyManager = manager;
    }

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    private void Update()
    {
        if (!root.activeSelf) return;

        isSkipping = Input.GetKey(KeyCode.LeftControl);

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
    }

    public void ShowLine(StoryLine line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        speakerText.text = storyManager.GetCharacterName(line.characterID);

        ApplyInnerThought(line.isInnerThought);

        currentFullText = line.line;
        
        history.Add(line.line);
        historyIndex = history.Count;

        typingCoroutine = StartCoroutine(TypeLine(line.line));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        contentText.text = "";

        foreach (char c in text)
        {
            contentText.text += c;

            if (!isSkipping)
                yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
    }

    private void ShowPrevious()
    {
        if (historyIndex <= 0) return;

        historyIndex--;
        contentText.text = history[historyIndex];
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
}
