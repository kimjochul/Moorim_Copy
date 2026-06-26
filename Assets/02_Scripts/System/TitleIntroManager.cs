using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TitleIntroManager : MonoBehaviour
{
    [SerializeField] private GameObject teamLogo;

    [SerializeField] private TitleMenuAnimator menuAnimator;

    [SerializeField] private float logoDuration = 2f;
    [SerializeField] private float fadeDuration = 1f;
    
    private IEnumerator Start()
    {
        yield return PlaySequence();
    }
    
    private IEnumerator PlaySequence()
    {
        // 처음부터 화면을 검게
        LoadUIManager.Instance.SetFadeImmediate(true);

        // 팀 로고
        teamLogo.SetActive(true);

        yield return new WaitForSeconds(logoDuration);

        // 로고 제거
        teamLogo.SetActive(false);

        // 검은 화면 제거
        yield return LoadUIManager.Instance.FadeIn(fadeDuration);

        // 메인메뉴 애니메이션
        menuAnimator.Play();
    }
}
