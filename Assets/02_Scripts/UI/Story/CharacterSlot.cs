using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex; // 0: 왼쪽, 1: 중앙, 2: 오른쪽

    [SerializeField] private Image characterImage;

    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;

    private Coroutine iconCoroutine;
    
    private RectTransform rt;
    public Vector2 SlotPos => rt.anchoredPosition;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    
    public void SetCharacter(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        characterImage.sprite = sprite;
        gameObject.SetActive(true);
        
        HideEmotionIcon();
    }
    
    public void SetHidden(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        characterImage.sprite = sprite;

        gameObject.SetActive(false);
    }

    public void Clear()
    {
        currentCharacterID = -1;
        characterImage.sprite = null;
        
        HideEmotionIcon();
    }
    
    public void Appear(int characterID, Sprite sprite, Vector2 targetPos)
    {
        SetCharacter(characterID, sprite);
        
        StopAllCoroutines();
        StartCoroutine(AppearRoutine(targetPos));
    }

    public void Disappear(Vector2 startPos)
    {
        Clear();
        
        StopAllCoroutines();
        StartCoroutine(DisappearRoutine(startPos));
    }
    
    public void PlayEmotionIcon(Sprite icon)
    {
        if (iconCoroutine != null)
            StopCoroutine(iconCoroutine);

        iconCoroutine = StartCoroutine(EmotionIconRoutine(icon));
    }

    private IEnumerator EmotionIconRoutine(Sprite icon)
    {
        emotionIcon.sprite = icon;
        emotionIcon.gameObject.SetActive(true);

        Transform t = emotionIcon.transform;

        // 시작 상태
        t.localScale = Vector3.zero;
        Vector3 startPos = t.localPosition;
        Vector3 targetPos = startPos + Vector3.up * 50f;

        float time = 0;

        // 1. 팝업 + 상승
        while (time < 0.25f)
        {
            time += Time.deltaTime;
            float t01 = time / 0.25f;

            // 팝 효과 (살짝 튕김)
            float scale = Mathf.Lerp(0f, 1.2f, t01);
            t.localScale = new Vector3(scale, scale, scale);

            t.localPosition = Vector3.Lerp(startPos, targetPos, t01);

            yield return null;
        }

        // 살짝 줄어들면서 안정화
        t.localScale = Vector3.one;

        // 유지
        yield return new WaitForSeconds(0.7f);

        // 2. 사라지기
        time = 0;
        while (time < 0.2f)
        {
            time += Time.deltaTime;
            float t01 = time / 0.2f;

            t.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t01);
            yield return null;
        }

        HideEmotionIcon();
        t.localPosition = startPos; // 원위치
    }

    // 흔들기 (캐릭터)
    public IEnumerator Shake(float duration = 0.3f, float power = 0.1f)
    {
        Vector3 origin = rt.anchoredPosition;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-power, power);
            float y = Random.Range(-power, power);

            rt.anchoredPosition = origin + new Vector3(x, y, 0);

            yield return null;
        }

        rt.anchoredPosition = origin;
    }
    
    public void HideEmotionIcon()
    {
        emotionIcon.gameObject.SetActive(false);
    }

    public void SetColor(Color target, float duration = 0.2f)
    {
        characterImage.DOColor(target, duration);
    }
    
    IEnumerator AppearRoutine(Vector2 targetPos)
    {
        Vector2 start = targetPos + Vector2.left * 200f;

        rt.anchoredPosition = start;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            rt.anchoredPosition = Vector2.Lerp(start, targetPos, t);
            yield return null;
        }

        rt.anchoredPosition = targetPos;
    }
    
    IEnumerator DisappearRoutine(Vector2 startPos)
    {
        Vector2  end = startPos + Vector2.right * 200f;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            transform.localPosition = Vector3.Lerp(startPos, end, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
    
    public void Exit(Vector2 direction, float distance, float duration)
    {
        transform.DOKill();

        Vector2 start = rt.anchoredPosition;
        Vector2 target = start + direction * distance;

        rt.DOAnchorPos(target, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                Disappear(target);
            });
    }
    
    public void Enter(Vector2 targetPos, Vector2 direction, float distance, float duration, bool isUnknown)
    {
        gameObject.SetActive(true);
        
        transform.DOKill();

        // 시작 위치 (화면 밖)
        Vector3 start = targetPos + direction * distance;
        rt.anchoredPosition = start;

        if (isUnknown)
            SetColor(new Color(0.1f, 0.1f, 0.1f, 1f));
        else
            SetColor(Color.white);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
        seq.Join(characterImage.DOFade(1f, duration));

        seq.OnComplete(() =>{ });
    }
    
    public void PlayAttack(Vector2 dir, float power = 80f, float duration = 0.15f)
    {
        Vector2 origin = rt.anchoredPosition;
        
        Vector2 forward = origin + dir * power;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(forward, duration).SetEase(Ease.OutQuad));
        seq.Append(rt.DOAnchorPos(origin, duration).SetEase(Ease.InQuad));
    }
    
    public void PlayHit(Vector2 dir, float power = 30f, float duration = 0.2f)
    {
        Vector2 origin = rt.anchoredPosition;

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(origin + dir * power, duration * 0.3f));
        seq.Append(rt.DOAnchorPos(origin, duration * 0.7f));

        // 흔들림
        rt.DOShakeAnchorPos(duration, 10f, 20, 90, false, true);
    }
    
    public void Emphasis(float strength = 0.2f)
    {
        rt.DOPunchScale(Vector3.one * strength, 0.3f, 10, 1);
    }
    
    public void NervousShake()
    {
        rt.DOShakeAnchorPos(0.5f, 5f, 30);
    }
    
    public void ClearImmediate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        currentCharacterID = -1;
        characterImage.sprite = null;

        HideEmotionIcon();
    }
}
