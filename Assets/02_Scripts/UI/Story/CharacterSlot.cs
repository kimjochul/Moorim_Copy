using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex; // 0, 1, 2, 3

    [SerializeField] private Image currentImage;
    [SerializeField] private Image nextImage;
    
    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;

    private Coroutine iconCoroutine;
    
    private RectTransform rt;
    public Vector2 SlotPos => rt.anchoredPosition;

    private Coroutine motionCoroutine;
    public bool IsPlayingMotion { get; private set; }
    
    public Emotion CurrentEmotion { get; private set; }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        
        currentImage.color = Color.white;
        nextImage.color = new Color(1,1,1,0);
    }
    
    public void SetCharacter(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        currentImage.sprite = sprite;
        CurrentEmotion = Emotion.Neutral;
        
        currentImage.color = Color.white;

        nextImage.color = new Color(1,1,1,0);
        
        gameObject.SetActive(true);
        
        HideEmotionIcon();
    }
    
    public void SetHidden(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        currentImage.sprite = sprite;
        nextImage.color = new Color(1,1,1,0);
        gameObject.SetActive(false);
    }

    public void Clear()
    {
        currentCharacterID = -1;
        currentImage.sprite = null;
        
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
    
    public void ChangeExpression(Sprite sprite, float duration = 0.2f)
    {
        rt.DOKill();
        currentImage.DOKill();
        nextImage.DOKill();
        
        Color keepColor = currentImage.color;
        nextImage.sprite = sprite;
        
        nextImage.color = new Color(keepColor.r, keepColor.g, keepColor.b, 0f);

        Sequence seq = DOTween.Sequence();

        seq.Append(nextImage.DOFade(1f, duration));
        seq.Join(currentImage.DOFade(0f, duration));

        seq.OnComplete(() =>
        {
            currentImage.sprite = sprite;
            currentImage.color = keepColor;
            nextImage.color = new Color(keepColor.r, keepColor.g, keepColor.b, 0f);
            
            rt.DOScale(1.03f, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    rt.DOScale(1f, 0.25f)
                        .SetEase(Ease.InOutQuad);
                });
        });
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
        currentImage.DOColor(target, duration);

        Color next = nextImage.color;
        next.r = target.r;
        next.g = target.g;
        next.b = target.b;
        nextImage.DOColor(next, duration);
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

            rt.anchoredPosition = Vector2.Lerp(startPos, end, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
    
    public IEnumerator ExitRoutine(Vector2 direction, float distance, float duration)
    {
        transform.DOKill();

        Vector2 start = rt.anchoredPosition;
        Vector2 target = start + direction * distance;

        Tween tween = rt.DOAnchorPos(target, duration).SetEase(Ease.InOutQuad);
        
        yield return tween.WaitForCompletion();
        
        Disappear(target);
    }
    
    public IEnumerator EnterRoutine(Vector2 targetPos, Vector2 direction, float distance, float duration, bool isUnknown)
    {
        gameObject.SetActive(true);
        transform.DOKill();

        nextImage.color = new Color(1,1,1,0);
        
        // 시작 위치 (화면 밖)
        Vector3 start = targetPos + direction * distance;
        rt.anchoredPosition = start;

        if (isUnknown)
            SetColor(new Color(0.1f, 0.1f, 0.1f, 1f));
        else
            SetColor(Color.white);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
        seq.Join(currentImage.DOFade(1f, duration));

        yield return seq.WaitForCompletion();
    }
    
    public IEnumerator PlayAttack(Vector2 dir, float power = 80f, float duration = 0.15f)
    {
        Vector2 origin = rt.anchoredPosition;
        
        Vector2 forward = origin + dir * power;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(forward, duration).SetEase(Ease.OutQuad));
        seq.Append(rt.DOAnchorPos(origin, duration).SetEase(Ease.InQuad));
        
        yield return seq.WaitForCompletion();
    }
    
    public IEnumerator PlayHit(Vector2 dir, float power = 30f, float duration = 0.2f)
    {
        Vector2 origin = rt.anchoredPosition;
        Debug.Log("Hit");
        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOAnchorPos(origin + dir * power, duration * 0.3f));
        seq.Append(rt.DOAnchorPos(origin, duration * 0.7f));

        // 흔들림
        rt.DOShakeAnchorPos(duration, 10f, 20, 90, false, true);
        
        yield return seq.WaitForCompletion();
    }
    
    public IEnumerator EmphasisRoutine(float strength = 0.2f)
    {
        Tween tween = rt.DOPunchScale(Vector3.one * strength, 0.3f, 10, 1);
        
        yield return tween.WaitForCompletion();
    }
    
    public IEnumerator NervousRoutine()
    {
        Tween tween = rt.DOShakeAnchorPos(0.5f, 5f, 30);
        
        yield return tween.WaitForCompletion();
    }
    
    public void ClearImmediate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        currentCharacterID = -1;
        currentImage.sprite = null;
        nextImage.sprite = null;
        nextImage.color = new Color(1,1,1,0);
        HideEmotionIcon();
    }
    
    public void SetEmotion(Emotion emotion)
    {
        CurrentEmotion = emotion;
    }
}
