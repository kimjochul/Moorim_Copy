using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex; // 0, 1, 2, 3

    [SerializeField] private RectTransform motionRoot;
    [SerializeField] private RectTransform visualRoot;
    [SerializeField] private Image currentImage;
    [SerializeField] private Image nextImage;
    
    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;

    private float currentBaseScale = 1f;
    
    private Coroutine iconCoroutine;
    
    private RectTransform slotRoot;
    private RectTransform SlotRoot
    {
        get
        {
            if (slotRoot == null)
                slotRoot = GetComponent<RectTransform>();

            return slotRoot;
        }
    }
    public RectTransform MotionRoot => motionRoot;
    public Vector2 SlotPos => SlotRoot.anchoredPosition;

    private Coroutine motionCoroutine;
    
    private int motionLock = 0;
    public bool IsPlayingMotion => motionLock > 0;

    
    private bool isUnknown;
    private bool isFocused;

    
    public Emotion CurrentEmotion { get; private set; }

    private void Awake()
    {
        slotRoot = GetComponent<RectTransform>();
        
        currentImage.color = Color.white;
        nextImage.color = new Color(1,1,1,0);
    }
    
    public void SetCharacter(int characterID, Sprite sprite)
    {
        HideEmotionIcon();
        
        visualRoot.DOKill();
        
        currentCharacterID = characterID;
        currentImage.sprite = sprite;
        CurrentEmotion = Emotion.Neutral;
        
        currentImage.color = Color.white;

        nextImage.color = new Color(1,1,1,0);
        
        visualRoot.localScale = Vector3.one * currentBaseScale;
        
        gameObject.SetActive(true);
    }
    
    public void SetHidden(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        currentImage.sprite = sprite;
        nextImage.color = new Color(1,1,1,0);
        visualRoot.localScale = Vector3.one * currentBaseScale;
        gameObject.SetActive(false);
    }

    public void SetBaseScale(float scale)
    {
        currentBaseScale = scale;
        visualRoot.localScale = Vector3.one * scale;
    }
    
    public void Clear()
    {
        currentCharacterID = -1;
        currentImage.sprite = null;
        
        HideEmotionIcon();
    }
    
    public void Appear(int characterID, Sprite sprite, Vector2 targetPos)
    {
        HideEmotionIcon();
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
    
    public void SetFocusScale(float focusMultiplier, float duration = 0.25f)
    {
        visualRoot.DOKill();

        visualRoot.DOScale(
            Vector3.one * currentBaseScale * focusMultiplier,
            duration);
    }
    
    public void ChangeExpression(Sprite sprite, float duration = 0.2f)
    {
        currentImage.DOKill();
        nextImage.DOKill();
        visualRoot.DOKill();
        
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
            
            visualRoot.DOScale(currentBaseScale * 1.03f, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    visualRoot.DOScale(currentBaseScale, 0.25f)
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
        BeginMotion();
        
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-power, power);
            float y = Random.Range(-power, power);

            motionRoot.anchoredPosition = new Vector2(x, y);

            yield return null;
        }

        motionRoot.anchoredPosition = Vector2.zero;
        
        EndMotion();
    }
    
    public void HideEmotionIcon()
    {
        if (iconCoroutine != null)
        {
            StopCoroutine(iconCoroutine);
            iconCoroutine = null;
        }

        emotionIcon.gameObject.SetActive(false);
    }

    public void SetUnknown(bool value)
    {
        isUnknown = value;
        RefreshColor();
    }
    
    public void SetFocused(bool value)
    {
        isFocused = value;
        RefreshColor();
    }

    private void RefreshColor()
    {
        Color target;

        if (isUnknown)
        {
            target = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
        else if (!isFocused)
        {
            target = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
        else
        {
            target = Color.white;
        }

        currentImage.DOColor(target, 0.2f);

        Color next = nextImage.color;
        next.r = target.r;
        next.g = target.g;
        next.b = target.b;

        nextImage.DOColor(next, 0.2f);
    }
    
    IEnumerator AppearRoutine(Vector2 targetPos)
    {
        BeginMotion();
        
        Vector2 startOffset = Vector2.left * 200f;
        motionRoot.anchoredPosition = startOffset;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            motionRoot.anchoredPosition = Vector2.Lerp(startOffset, Vector2.zero, t);
            yield return null;
        }

        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
    }
    
    IEnumerator DisappearRoutine(Vector2 startPos)
    {
        BeginMotion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        Vector2 endOffset = Vector2.right * 200f;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            motionRoot.anchoredPosition = Vector2.Lerp(startPos, endOffset, t);
            yield return null;
        }
        
        EndMotion();
    }
    
    public IEnumerator MoveRoutine(Vector2 dir, float distance, float duration)
    {
        BeginMotion();

        Vector2 target = dir * distance;

        Tween tween = motionRoot
            .DOAnchorPos(target, duration)
            .SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();

        motionRoot.anchoredPosition = Vector2.zero;

        EndMotion();
    }
    
    public IEnumerator MoveToPosition(Vector2 targetPos, float duration)
    {
        gameObject.SetActive(true);
        BeginMotion();
        motionRoot.DOKill();

        Debug.Log($"{slotIndex}번 슬롯 {targetPos}로 이동");
        Tween tween = slotRoot.DOAnchorPos(targetPos, duration).SetEase(Ease.InOutQuad);

        yield return tween.WaitForCompletion();

        EndMotion();
    }
    
    
    
    public IEnumerator ExitRoutine(Vector2 direction, float distance, float duration)
    {
        BeginMotion();
        
        motionRoot.DOKill();
        
        Vector2 targetOffset = direction * distance;

        Tween tween = motionRoot.DOAnchorPos(targetOffset, duration).SetEase(Ease.InOutQuad);
        
        yield return tween.WaitForCompletion();
        
        EndMotion();
        gameObject.SetActive(false);
    }
    
    public IEnumerator EnterRoutine(Vector2 targetPos, Vector2 direction, float distance, float duration, bool isUnknown)
    {
        gameObject.SetActive(true);
        BeginMotion();
        motionRoot.DOKill();

        nextImage.color = new Color(1,1,1,0);
        
        slotRoot.anchoredPosition = targetPos;
        // 시작 위치 (화면 밖)
        Vector2 startOffset = direction * distance;
        motionRoot.anchoredPosition = startOffset;

        SetUnknown(isUnknown);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(motionRoot.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic));
        seq.Join(currentImage.DOFade(1f, duration));

        yield return seq.WaitForCompletion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
    }
    
    public IEnumerator PlayAttack(Vector2 dir, float power = 80f, float duration = 0.15f)
    {
        BeginMotion();
        
        Vector2 forward = dir * power;

        Sequence seq = DOTween.Sequence();
        seq.Append(motionRoot.DOAnchorPos(forward, duration).SetEase(Ease.OutQuad));
        seq.Append(motionRoot.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InQuad));
        
        yield return seq.WaitForCompletion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
    }
    
    public IEnumerator PlayHit(Vector2 dir, float power = 30f, float duration = 0.2f)
    {
        BeginMotion();
        
        Debug.Log("Hit");
        Sequence seq = DOTween.Sequence();

        seq.Append(motionRoot.DOAnchorPos(dir * power, duration * 0.3f));
        seq.Append(motionRoot.DOAnchorPos(Vector2.zero, duration * 0.7f));

        // 흔들림
        motionRoot.DOShakeAnchorPos(duration, 10f, 20, 90, false, true);
        
        yield return seq.WaitForCompletion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
    }
    
    public IEnumerator JumpRoutine(float height, float duration)
    {
        BeginMotion();

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            float y = Mathf.Sin(t * Mathf.PI) * height;

            motionRoot.anchoredPosition =
                new Vector2(0, y);

            yield return null;
        }

        motionRoot.anchoredPosition = Vector2.zero;

        EndMotion();
    }
    
    public IEnumerator EmphasisRoutine(float strength = 0.2f)
    {
        BeginMotion();
        Tween tween = visualRoot.DOPunchScale(Vector3.one * strength, 0.3f, 10, 1);
        
        yield return tween.WaitForCompletion();
        EndMotion();
    }
    
    public IEnumerator NervousRoutine()
    {
        BeginMotion();
        Tween tween = motionRoot.DOShakeAnchorPos(0.5f, 5f, 30);
        
        yield return tween.WaitForCompletion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
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
    
    private void BeginMotion()
    {
        motionLock++;
    }

    private void EndMotion()
    {
        motionLock = Mathf.Max(0, motionLock - 1);
    }
}
