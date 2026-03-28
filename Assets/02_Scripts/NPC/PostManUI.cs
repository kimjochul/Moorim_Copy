using UnityEngine;

public class PostManUI : MonoBehaviour
{
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject talkPanel;

    private Postman postman;

    public void Init(Postman postman)
    {
        this.postman = postman;

        OpenTalk();
    }

    public void OpenQuest()
    {
        questPanel.SetActive(true);
        talkPanel.SetActive(false);
    }

    public void OpenTalk()
    {
        questPanel.SetActive(false);
        talkPanel.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
