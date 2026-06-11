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
    [SerializeField] private Image eyebrowImage;
    
    [SerializeField] private Image shadowImage;
    
    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;
    private Character currentCharacter;

    private bool isFlipped; 
    private float currentBaseScale = 1f;
    private float currentScaleMultiplier = 1f;
    
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
    
    public Vector2 SlotPos => SlotRoot.anchoredPosition;
    private Vector2 defaultEmotionIconPos;
    
    private int motionLock = 0;
    public bool IsPlayingMotion => motionLock > 0;

    private bool isUnknown;
    private bool isFocused;
    
    private FaceType currentFace = FaceType.Neutral;
    private EyebrowType currentEyebrow = EyebrowType.Default;
    private bool currentShadow = false;
    
    private bool HasFaceLayer => currentCharacter != null && currentCharacter.faces != null && currentCharacter.faces.Count > 0;
    private bool HasEyebrowLayer => currentCharacter != null && currentCharacter.eyebrows != null && currentCharacter.eyebrows.Count > 0;
    
    private void Awake()
    {
        slotRoot = GetComponent<RectTransform>();
        
        characterImage.color = Color.white;
        defaultEmotionIconPos = emotionIcon.rectTransform.anchoredPosition;
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

        if (character.shadow != null)
        {
            currentShadow = false;
            shadowImage.sprite = character.shadow;
        }

        if (character.visual != null)
        {
            emotionIcon.rectTransform.anchoredPosition = character.visual.emotionIconPosition;
        }
        else
        {
            emotionIcon.rectTransform.anchoredPosition = defaultEmotionIconPos;
        }
        
        RefreshShadow();
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
        
        if (character.shadow != null)
        {
            currentShadow = false;
            shadowImage.sprite = character.shadow;
        }
        
        if (character.visual != null)
        {
            emotionIcon.rectTransform.anchoredPosition = character.visual.emotionIconPosition;
        }
        else
        {
            emotionIcon.rectTransform.anchoredPosition = defaultEmotionIconPos;
        }
        
        RefreshShadow();
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
        eyebrowImage.sprite = null;
        
        faceImage.color = Color.white;
        eyebrowImage.color = Color.white;

        currentShadow = false;
        shadowImage.sprite = null;
        RefreshShadow();
        
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
    
    private void ChangeExpression(Sprite faceSprite, Sprite eyebrowSprite, float duration = 0.2f)
    {
        bool changeFace = HasFaceLayer && faceSprite != faceImage.sprite;
        bool changeEyebrow = HasEyebrowLayer && eyebrowSprite != eyebrowImage.sprite;

        if (!changeFace && !changeEyebrow)
            return;

        if (changeFace)
            faceImage.sprite = faceSprite;

        if (changeEyebrow)
            eyebrowImage.sprite = eyebrowSprite;

        RefreshColor();
        
        PlayExpressionScale();

        Debug.Log($"표정 변경 {faceSprite?.name} 눈썹 변경 {eyebrowSprite?.name}");
    }
    
    private void PlayExpressionScale()
    {
        visualRoot.DOComplete();
        
        float scale = currentBaseScale * currentScaleMultiplier;
        float x = isFlipped ? -scale : scale;

        visualRoot.DOScale(new Vector3(x * 1.01f, scale * 1.01f, 1f), 0.25f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                visualRoot.DOScale(new Vector3(x, scale, 1f), 0.25f).SetEase(Ease.InOutQuad);
            });
    }
    
    public void PlayEmotionIcon(Sprite icon)
    {
        emotionIcon.DOKill();
        
        emotionIcon.sprite = icon;
        emotionIcon.gameObject.SetActive(true);

        RectTransform rt = emotionIcon.rectTransform;

        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * 50f;

        Color c = emotionIcon.color;
        c.a = 0;
        emotionIcon.color = c;
        
        Sequence seq = DOTween.Sequence();
        
        // 등장
        seq.Append(emotionIcon.DOFade(1f, 0.15f));
        seq.Join(rt.DOAnchorPos(endPos, 0.3f).SetEase(Ease.OutQuad));

        // 유지
        seq.AppendInterval(0.8f);

        // 퇴장
        seq.Append(emotionIcon.DOFade(0f, 0.2f));
        seq.Join(rt.DOAnchorPos(endPos + Vector2.down * 20f, 0.2f));
        
        seq.OnComplete(() =>
        {
            rt.anchoredPosition = startPos;
            HideEmotionIcon();
        });
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
    
    private void HideEmotionIcon()
    {
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
        Color targetForShadow;
        // #76564D98
        if (isUnknown)
        {
            target = new Color(0.1f, 0.1f, 0.1f, 1f);
            targetForShadow = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        }
        else if (!isFocused)
        {
            target = new Color(0.7f, 0.7f, 0.7f, 1f);
            targetForShadow = new Color(0.35f, 0.35f, 0.35f, 0.5f);
        }
        else
        {
            target = Color.white;
            targetForShadow = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        characterImage.DOColor(target, 0.2f);
        faceImage.DOColor(target,0.2f);
        eyebrowImage.DOColor(target,0.2f);
        
        if (shadowImage.gameObject.activeSelf)
            shadowImage.DOColor(targetForShadow, 0.2f);
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
        eyebrowImage.sprite = null;
        
        faceImage.color = Color.white;
        eyebrowImage.color = Color.white;

        characterImage.color = Color.white;
        
        currentShadow = false;
        shadowImage.sprite = null;
        RefreshShadow();
        
        HideEmotionIcon();
        
        isUnknown = false;
        isFocused = false;
        
        isFlipped = false;
        currentScaleMultiplier = 1f;
        currentBaseScale = 1f;
        RefreshScale();
    }
    
    public void SetExpression(ExpressionData expression)
    {
        if (expression == null || currentCharacter == null)
            return;
        
        FaceType nextFace = currentFace;
        EyebrowType nextEyebrow = currentEyebrow;
        bool nextShadow = currentShadow;
        
        if (expression.face != FaceType.None)
            nextFace = expression.face;

        if (expression.eyebrow != EyebrowType.None)
            nextEyebrow = expression.eyebrow;

        switch (expression.shadow)
        {
            case ShadowType.On:
                nextShadow = true;
                break;

            case ShadowType.Off:
                nextShadow = false;
                break;

            // None이면 그대로 유지
        }
        
        if (nextFace == currentFace && nextEyebrow == currentEyebrow && nextShadow == currentShadow)
            return;

        bool shadowChanged = nextShadow != currentShadow;
        
        currentFace = nextFace;
        currentEyebrow = nextEyebrow;
        currentShadow = nextShadow;
        
        if (shadowChanged)
            ChangeShadow(currentShadow);
        
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

        if (showFace)
            faceImage.sprite = faceSprite;

        // 눈썹
        bool showEyebrow = HasEyebrowLayer && eyebrowSprite != null;

        eyebrowImage.gameObject.SetActive(showEyebrow);

        if (showEyebrow)
            eyebrowImage.sprite = eyebrowSprite;
    }
    
    private void RefreshShadow()
    {
        shadowImage.DOKill();

        if (currentShadow)
        {
            shadowImage.gameObject.SetActive(true);

            RefreshColor(); // 현재 focus 상태 색 적용
        }
        else
        {
            shadowImage.gameObject.SetActive(false);
        }
    }
    
    private void ChangeShadow(bool showShadow, float duration = 0.2f)
    {
        shadowImage.DOKill();

        if (showShadow)
        {
            shadowImage.gameObject.SetActive(true);

            RefreshColor();

            shadowImage.DOFade(0.5f, duration);
        }
        else
        {
            shadowImage.DOFade(0f, duration)
                .OnComplete(() =>
                {
                    shadowImage.gameObject.SetActive(false);

                    RefreshColor();
                });
        }
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
