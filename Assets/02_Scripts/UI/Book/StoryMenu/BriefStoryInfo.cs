using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BriefStoryInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtDescription;

    [SerializeField] private RectTransform popupRect;
    
    private const int MaxDescriptionLength = 300;
    private const string Suffix = "\n...(후략)";
    
    void Start()
    {
        HideInfo();
    }

    public void SetInfo(Story story)
    {
        txtName.text = story.storyName;
        SetDescription(story.description);
    }

    private void SetDescription(string description)
    {
        if (description.Length <= MaxDescriptionLength)
        {
            txtDescription.text = description;
            return;
        }

        // 300자 이내에서 마지막 \n\n 위치 찾기
        int cutIndex = description.LastIndexOf("\n", MaxDescriptionLength, StringComparison.Ordinal);

        if (cutIndex <= 0)
        {
            // 300자 안에 \n\n이 없으면 그냥 300자에서 자름
            txtDescription.text = description.Substring(0, MaxDescriptionLength) + Suffix;
            return;
        }

        // \n\n 앞까지만 출력
        txtDescription.text = description.Substring(0, cutIndex) + Suffix;
    }
    
    public void ShowInfo(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        // 오른쪽 중앙
        Vector3 rightCenter = (corners[2] + corners[3]) * 0.5f;

        popupRect.position = rightCenter + Vector3.right * 250f + Vector3.down * 80;

        gameObject.SetActive(true);
    }
    
    public void HideInfo()
    {
        this.gameObject.SetActive(false);
    }
}
