using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadUIManager : Singleton<LoadUIManager>
{
    [SerializeField] private CanvasGroup fade;
    [SerializeField] private GameObject loadingUI;

    [Header("Loading UI")]
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private TextMeshProUGUI tipText;

    private float currentProgress = 0f;

    #region Fade

    public IEnumerator FadeOut(float duration = 0.5f)
    {
        fade.gameObject.SetActive(true);
        fade.blocksRaycasts = true;

        yield return fade.DOFade(1f, duration).WaitForCompletion();
    }

    public IEnumerator FadeIn(float duration = 0.5f)
    {
        yield return fade.DOFade(0f, duration).WaitForCompletion();

        fade.blocksRaycasts = false;
        fade.gameObject.SetActive(false);
    }

    #endregion

    #region Loading UI

    public void ShowLoading()
    {
        loadingUI.SetActive(true);
        currentProgress = 0f;
        UpdateUI(0f);
    }

    public void HideLoading()
    {
        loadingUI.SetActive(false);
    }

    public void SetProgress(float target)
    {
        // 🔥 Lerp 적용 (부드럽게)
        currentProgress = Mathf.Lerp(currentProgress, target, Time.deltaTime * 5f);
        UpdateUI(currentProgress);
    }

    private void UpdateUI(float value)
    {
        progressBar.fillAmount = value;
        percentText.text = $"{(int)(value * 100)}%";
        
        if (value < 0.3f)
            tipText.text = "객잔을 정리하는 중...";
        else if (value < 0.7f)
            tipText.text = "손님 맞을 준비 중...";
        else
            tipText.text = "가게 문을 여는 중...";
    }
    
    public void ForceProgress(float value)
    {
        currentProgress = value;
        UpdateUI(value);
    }

    #endregion
}
