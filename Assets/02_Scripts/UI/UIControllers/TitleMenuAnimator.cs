using System;
using DG.Tweening;
using UnityEngine;

public class TitleMenuAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform logo;
    [SerializeField] private CanvasGroup logoGroup;

    [SerializeField] private RectTransform buttonGroup;
    [SerializeField] private CanvasGroup buttonCanvas;

    [SerializeField] private float logoMove = 80f;
    [SerializeField] private float buttonMove = 60f;

    private void Start()
    {
        logo.gameObject.SetActive(false);
        buttonGroup.gameObject.SetActive(false);
    }

    public void Play()
    {
        logoGroup.alpha = 0;
        buttonCanvas.alpha = 0;

        Vector2 logoPos = logo.anchoredPosition;
        Vector2 buttonPos = buttonGroup.anchoredPosition;
        
        logo.anchoredPosition = logoPos + Vector2.left * logoMove;
        buttonGroup.anchoredPosition = buttonPos - Vector2.up * buttonMove;

        logo.gameObject.SetActive(true);
        buttonGroup.gameObject.SetActive(true);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(logoGroup.DOFade(1, 1.5f));
        seq.Join(logo.DOAnchorPos(logoPos, 1.5f).SetEase(Ease.OutBack));
        
        seq.AppendInterval(0.2f);
        
        seq.Append(buttonCanvas.DOFade(1, 1f));
        seq.Join(buttonGroup.DOAnchorPos(buttonPos, 1f).SetEase(Ease.OutCubic));
    }
}
