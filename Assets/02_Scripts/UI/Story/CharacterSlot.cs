using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex; // 0, 1, 2, 3

    [SerializeField] private RectTransform motionRoot;
    [SerializeField] private RectTransform visualRoot;
    
    [SerializeField] private Image characterImage;
    
    [SerializeField] private Image faceImage;
    [SerializeField] private Image nextFaceImage;

    [SerializeField] private Image eyebrowImage;
    [SerializeField] private Image nextEyebrowImage;
    
    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;
    private Character currentCharacter;

    private bool isFlipped;
    private float currentBaseScale = 1f;
    private float currentScaleMultiplier = 1f;
    
    private Coroutine iconCoroutine;
    
    private RectTransform slotRoot;
    public RectTransform SlotRoot
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
    
    private FaceType currentFace = FaceType.Neutral;
    private EyebrowType currentEyebrow = EyebrowType.Default;
    public FaceType CurrentFace => currentFace;
    public EyebrowType CurrentEyebrow => currentEyebrow;
    
    private bool HasFaceLayer =>
        currentCharacter != null &&
        currentCharacter.faces != null &&
        currentCharacter.faces.Count > 0;

    private bool HasEyebrowLayer =>
        currentCharacter != null &&
        currentCharacter.eyebrows != null &&
        currentCharacter.eyebrows.Count > 0;
    
    private void Awake()
    {
        slotRoot = GetComponent<RectTransform>();
        
        characterImage.color = Color.white;
    }
    
    public void SetCharacter(Character character, Sprite bodySprite)
    {
        HideEmotionIcon();
        
        visualRoot.DOKill();
        
        currentCharacter = character;
        currentCharacterID = character.characterID;
        characterImage.sprite = bodySprite;
        
        currentFace = FaceType.Neutral;
        currentEyebrow = EyebrowType.Default;
        
        characterImage.color = Color.white;

        faceImage.color = Color.white;
        eyebrowImage.color = Color.white;
        nextFaceImage.color = new Color(1,1,1,0);
        nextEyebrowImage.color = new Color(1,1,1,0);
        
        RefreshExpression();
        
        isUnknown = false;
        isFocused = false;
        RefreshScale();
        
        gameObject.SetActive(true);
    }
    
    public void SetHidden(Character character, Sprite bodySprite)
    {
        currentCharacter = character;
        currentCharacterID = character.characterID;
        characterImage.sprite = bodySprite;
        
        currentFace = FaceType.Neutral;
        currentEyebrow = EyebrowType.Default;
        RefreshExpression();
        
        RefreshScale();
        gameObject.SetActive(false);
    }

    public void SetFlip(bool flip)
    {
        isFlipped = flip;
        RefreshScale();
    }
    
    public void SetBaseScale(float scale)
    {
        currentBaseScale = scale;
        RefreshScale();
    }
    
    private void RefreshScale()
    {
        float x = currentBaseScale * currentScaleMultiplier;

        if (isFlipped)
            x *= -1f;

        visualRoot.localScale = new Vector3(x, currentBaseScale * currentScaleMultiplier, 1f);
    }
    
    private void Clear()
    {
        currentCharacter = null;
        currentCharacterID = -1;
        characterImage.sprite = null;
        
        faceImage.sprite = null;
        nextFaceImage.sprite = null;

        eyebrowImage.sprite = null;
        nextEyebrowImage.sprite = null;
        
        faceImage.color = Color.white;
        eyebrowImage.color = Color.white;

        nextFaceImage.color = new Color(1,1,1,0);
        nextEyebrowImage.color = new Color(1,1,1,0);
        
        HideEmotionIcon();
    }
    
    public void Appear(Character character, Sprite bodySprite, Vector2 targetPos)
    {
        HideEmotionIcon();
        SetCharacter(character, bodySprite);
        
        StopAllCoroutines();
        StartCoroutine(AppearRoutine(targetPos));
    }

    public void Disappear(Vector2 startPos)
    {
        if (!gameObject.activeInHierarchy)
            return;
        
        Clear();
        
        StopAllCoroutines();
        StartCoroutine(DisappearRoutine(startPos));
    }
    
    public void SetFocusScale(float focusMultiplier, float duration = 0.25f)
    {
        visualRoot.DOKill();
        
        currentScaleMultiplier = focusMultiplier;
        float targetX = currentBaseScale * focusMultiplier;
        
        if (isFlipped)
            targetX *= -1f;
        
        visualRoot.DOScale(new Vector3(targetX, currentBaseScale * focusMultiplier, 1f), duration);
    }
    
    public void ChangeExpression(Sprite faceSprite, Sprite eyebrowSprite, float duration = 0.2f)
    {
        faceImage.DOKill();
        nextFaceImage.DOKill();
        eyebrowImage.DOKill();
        nextEyebrowImage.DOKill();
        visualRoot.DOKill();
        
        Color faceColor = faceImage.color;
        Color eyebrowColor = eyebrowImage.color;
        
        bool changeFace = HasFaceLayer;
        bool changeEyebrow = HasEyebrowLayer;

        if (changeFace)
        {
            nextFaceImage.sprite = faceSprite;
            nextFaceImage.color = new Color(faceColor.r, faceColor.g, faceColor.b, 0);
        }

        if (changeEyebrow)
        {
            nextEyebrowImage.sprite = eyebrowSprite;
            nextEyebrowImage.color = new Color(eyebrowColor.r, eyebrowColor.g, eyebrowColor.b, 0);
        }

        Sequence seq = DOTween.Sequence();

        if(changeFace)
        {
            seq.Join(nextFaceImage.DOFade(1, duration));
            seq.Join(faceImage.DOFade(0, duration));
        }
        
        if(changeEyebrow)
        {
            seq.Join(nextEyebrowImage.DOFade(1, duration));
            seq.Join(eyebrowImage.DOFade(0, duration));
        }
        
        seq.OnComplete(() =>
        {
            if (changeFace)
            {
                faceImage.sprite = faceSprite;
                faceImage.color = faceColor;

                nextFaceImage.sprite = null;
                nextFaceImage.color =
                    new Color(faceColor.r, faceColor.g, faceColor.b, 0);
            }

            if (changeEyebrow)
            {
                eyebrowImage.sprite = eyebrowSprite;
                eyebrowImage.color = eyebrowColor;

                nextEyebrowImage.sprite = null;
                nextEyebrowImage.color =
                    new Color(eyebrowColor.r, eyebrowColor.g, eyebrowColor.b, 0);
            }
            
            PlayExpressionScale();
        });
    }
    
    private void PlayExpressionScale()
    {
        float scale = currentBaseScale * currentScaleMultiplier;

        float x = isFlipped ? -scale : scale;

        visualRoot.DOScale(new Vector3(x * 1.03f, scale * 1.03f, 1f), 0.25f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                visualRoot.DOScale(new Vector3(x, scale, 1f), 0.25f).SetEase(Ease.InOutQuad);
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
        
        Vector2 originalPos = motionRoot.anchoredPosition;
        
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-power, power);
            float y = Random.Range(-power, power);

            motionRoot.anchoredPosition = originalPos + new Vector2(x, y);

            yield return null;
        }

        motionRoot.anchoredPosition = originalPos;
        
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

        characterImage.DOColor(target, 0.2f);
        faceImage.DOColor(target,0.2f);
        eyebrowImage.DOColor(target,0.2f);

        Color nextFace = nextFaceImage.color;
        nextFace.r = target.r;
        nextFace.g = target.g;
        nextFace.b = target.b;

        nextFaceImage.DOColor(nextFace,0.2f);

        Color nextEyebrow = nextEyebrowImage.color;
        nextEyebrow.r = target.r;
        nextEyebrow.g = target.g;
        nextEyebrow.b = target.b;

        nextEyebrowImage.DOColor(nextEyebrow,0.2f);
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
        gameObject.SetActive(false);
    }
    
    public IEnumerator MoveRoutine(Vector2 dir, float distance, float duration)
    {
        BeginMotion();

        Vector2 target = dir * distance;

        Tween tween = motionRoot
            .DOAnchorPos(target, duration)
            .SetEase(Ease.OutQuad);

        yield return tween.WaitForCompletion();

        slotRoot.anchoredPosition += target; 
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
        
        slotRoot.anchoredPosition = targetPos;
        // 시작 위치 (화면 밖)
        Vector2 startOffset = direction * distance;
        motionRoot.anchoredPosition = startOffset;

        SetUnknown(isUnknown);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(motionRoot.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic));
        //seq.Join(currentImage.DOFade(1f, duration));

        yield return seq.WaitForCompletion();
        
        motionRoot.anchoredPosition = Vector2.zero;
        EndMotion();
    }
    
    public IEnumerator EnterMoveRoutine(float startOffset, Vector2 targetPos, float duration, bool isUnknown)
    {
        gameObject.SetActive(true);
        BeginMotion();
        slotRoot.DOKill();
        
        slotRoot.anchoredPosition = targetPos;
        Vector2 startPos = new Vector2(targetPos.x + startOffset, motionRoot.anchoredPosition.y);
        motionRoot.anchoredPosition = startPos;

        Color c = characterImage.color;
        c.a = 0f;
        characterImage.color = c;

        SetUnknown(isUnknown);
        
        Sequence seq = DOTween.Sequence();

        seq.Append(motionRoot.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutCubic));
        //seq.Join(currentImage.DOFade(1f, duration));

        yield return seq.WaitForCompletion();
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
        RefreshScale();
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
    
    public IEnumerator SitRoutine(float distance = 80f, float duration = 0.25f)
    {
        BeginMotion();
        motionRoot.DOKill();

        Tween tween = motionRoot.DOAnchorPosY(-distance, duration).SetEase(Ease.OutQuad);
        yield return tween.WaitForCompletion();

        EndMotion();
    }

    public IEnumerator StandRoutine(float duration = 0.25f)
    {
        BeginMotion();
        motionRoot.DOKill();

        Tween tween = motionRoot.DOAnchorPosY(0f, duration).SetEase(Ease.OutQuad);
        yield return tween.WaitForCompletion();

        EndMotion();
    }
    
    public void ClearImmediate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        
        currentCharacter = null;
        currentCharacterID = -1;
        
        characterImage.sprite = null;
        faceImage.sprite = null;
        nextFaceImage.sprite = null;
        eyebrowImage.sprite = null;
        nextEyebrowImage.sprite = null;
        
        faceImage.color = Color.white;
        eyebrowImage.color = Color.white;

        nextFaceImage.color = new Color(1,1,1,0);
        nextEyebrowImage.color = new Color(1,1,1,0);

        characterImage.color = Color.white;
        
        HideEmotionIcon();
        
        isUnknown = false;
        isFocused = false;
        
        isFlipped = false;
        currentScaleMultiplier = 1f;
        currentBaseScale = 1f;
        RefreshScale();
    }
    
    public void SetEmotion(Emotion emotion)
    {
        CurrentEmotion = emotion;
    }
    
    public void SetExpression(FaceType face = FaceType.None, EyebrowType eyebrow = EyebrowType.None)
    {
        FaceType nextFace = currentFace;
        EyebrowType nextEyebrow = currentEyebrow;

        if (face != FaceType.None)
            nextFace = face;

        if (eyebrow != EyebrowType.None)
            nextEyebrow = eyebrow;

        if (nextFace == currentFace && nextEyebrow == currentEyebrow)
            return;

        currentFace = nextFace;
        currentEyebrow = nextEyebrow;

        Sprite faceSprite = null;
        Sprite eyebrowSprite = null;

        if (HasFaceLayer)
            faceSprite = currentCharacter.faces.Find(x => x.type == currentFace)?.sprite;

        if (HasEyebrowLayer)
            eyebrowSprite = currentCharacter.eyebrows.Find(x => x.type == currentEyebrow)?.sprite;

        // 얼굴, 눈썹 레이어가 하나도 없으면 애니메이션 자체를 안 함
        if (faceSprite == null && eyebrowSprite == null)
        {
            RefreshExpression();
            return;
        }

        ChangeExpression(faceSprite, eyebrowSprite);
    }
    
    private void RefreshExpression()
    {
        Sprite faceSprite = null;
        Sprite eyebrowSprite = null;

        if (HasFaceLayer)
            faceSprite = currentCharacter.faces.Find(x => x.type == currentFace)?.sprite;

        if (HasEyebrowLayer)
            eyebrowSprite = currentCharacter.eyebrows.Find(x => x.type == currentEyebrow)?.sprite;

        // 얼굴
        bool showFace = HasFaceLayer && faceSprite != null;

        faceImage.gameObject.SetActive(showFace);
        nextFaceImage.gameObject.SetActive(showFace);

        if (showFace)
            faceImage.sprite = faceSprite;

        // 눈썹
        bool showEyebrow = HasEyebrowLayer && eyebrowSprite != null;

        eyebrowImage.gameObject.SetActive(showEyebrow);
        nextEyebrowImage.gameObject.SetActive(showEyebrow);

        if (showEyebrow)
            eyebrowImage.sprite = eyebrowSprite;
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
