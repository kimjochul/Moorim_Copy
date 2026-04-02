using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public enum OrderDirection
{
    Top,
    Bottom,
    Left,
    Right
}
public enum PaperDirection
{
    TopLeft,      // 왼쪽 위
    TopRight,     // 오른쪽 위
    BottomLeft,   // 왼쪽 아래
    BottomRight   // 오른쪽 아래
}

public class UIAnimationManager : Singleton<UIAnimationManager>
{
    [Header("=== 점점 커지는 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float popShowDuration = 0.3f;
    [Range(0f, 1f)]
    [SerializeField]
    private float popStartScale = 0.5f;
    [SerializeField]
    private Ease popShowEase = Ease.OutBack;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float popHideDuration = 0.2f;
    [Range(0f, 1f)]
    [SerializeField]
    private float popEndScale = 0.0f;
    [SerializeField]
    private Ease popHideEase = Ease.InBack;

    [Space(30)]

    [Header("=== 말풍선 펼치기 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float orderShowDuration = 0.3f;
    [SerializeField]
    private Ease orderShowEase = Ease.OutBack;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float orderHideDuration = 0.2f;
    [SerializeField]
    private Ease orderHideEase = Ease.InBack;

    [Space(30)]

    [Header("=== 세로 펼치기 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float vUnfoldShowDuration = 0.3f;
    [SerializeField]
    private Ease vUnfoldShowEase = Ease.OutBack;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float vUnfoldHideDuration = 0.2f;
    [SerializeField]
    private Ease vUnfoldHideEase = Ease.InBack;

    [Space(30)]

    [Header("=== 가로 펼치기 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float hUnfoldShowDuration = 0.3f;
    [SerializeField]
    private Ease hUnfoldShowEase = Ease.OutBack;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float hUnfoldHideDuration = 0.2f;
    [SerializeField]
    private Ease hUnfoldHideEase = Ease.InBack;

    [Space(30)]

    [Header("=== 종이 펼치기 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float paperShowDuration = 0.5f;
    [SerializeField]
    private Ease paperShowEase = Ease.OutQuart;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float paperHideDuration = 0.3f;
    [SerializeField]
    private Ease paperHideEase = Ease.InQuart;

    [Space(30)]

    [Header("=== 위에서 떨어져 통통 튀는 연출 ===")]
    [Header("Show 설정")]
    [Range(0.1f, 1.5f)]
    [SerializeField] 
    private float bouncePopShowDuration = 0.8f;
    [SerializeField]
    private Ease bouncePopShowEase = Ease.OutBounce;

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float bounceHideDuration = 0.3f;
    [SerializeField]
    private Ease bounceHideEase = Ease.InBack;

    [Tooltip("기존 위치에서 얼마나 위쪽에서 떨어질지 설정")]
    [SerializeField] 
    private float bouncePopStartOffset = 1000f;

    [Space(30)]

    [Header("=== 지그재그 바운스 연출 ===")]
    [Header("Show 설정")]
    [Range(0.5f, 2f)]
    [SerializeField] 
    private float zigzagShowDuration = 1.2f; // 튕기며 이동할 충분한 시간

    [Header("Hide 설정")]
    [Range(0.1f, 1f)]
    [SerializeField]
    private float zigzagHideDuration = 0.3f;
    [SerializeField]
    private Ease zigzagHideEase = Ease.InBack;

    [Tooltip("시작할 때 높이 값")]
    [SerializeField] 
    private float zigzagDropHeight = 800f;
    [Tooltip("시작할 때 왼쪽 값")]
    [SerializeField] 
    private float zigzagLeftOffset = 400f;
    [Tooltip("왼쪽에서 튕긴 후 오른쪽 값")]
    [SerializeField] 
    private float zigzagRightOffset = 200f;

    [Space(30)]

    [Header("=== Stamp 연출 ===")]
    [Header("Show 설정")]
    [SerializeField] 
    private Vector2 stampStartScale = new Vector2(10f, 10f); 
    [SerializeField] 
    private float stampAnimDuration = 0.5f;
    [SerializeField] 
    private Ease stampEase = Ease.OutBack;
    protected override void Awake()
    {
        base.Awake();

        DOTween.Init();
    }
    public void ShowPop(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        // 1. 기존 트윈 끄기
        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        ui.localScale = Vector3.one * this.popStartScale;

        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.one, this.popShowDuration).SetEase(this.popShowEase));
        seq.Join(cg.DOFade(1f, this.popShowDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HidePop(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.one * this.popEndScale, this.popHideDuration).SetEase(this.popHideEase));
        seq.Join(cg.DOFade(0f, this.popHideDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            onComplete?.Invoke();
        });
    }
    public void ShowOrder(RectTransform ui, OrderDirection direction = OrderDirection.Right, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();
        cg.blocksRaycasts = false;

        bool isVertical = (direction == OrderDirection.Top || direction == OrderDirection.Bottom);

        ui.localScale = isVertical ? new Vector3(1f, 0f, 1f) : new Vector3(0f, 1f, 1f);

        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        Vector2 startPivot = new Vector2(0.5f, 0.5f);
        switch (direction)
        {
            case OrderDirection.Top: startPivot = new Vector2(0.5f, 1f); break; // 위쪽 고정
            case OrderDirection.Bottom: startPivot = new Vector2(0.5f, 0f); break; // 아래쪽 고정
            case OrderDirection.Left: startPivot = new Vector2(0f, 0.5f); break; // 왼쪽 고정
            case OrderDirection.Right: startPivot = new Vector2(1f, 0.5f); break; // 오른쪽 고정
        }

        // 시작할 때
        seq.OnStart(() => {
            SetPivot(ui, startPivot);
            onStart?.Invoke();
        });

        if (isVertical)
        {
            seq.Append(ui.DOScaleY(1f, this.orderShowDuration).SetEase(this.orderShowEase));
        }
        else
        {
            seq.Append(ui.DOScaleX(1f, this.orderShowDuration).SetEase(this.orderShowEase));
        }

        seq.Join(cg.DOFade(1f, this.orderShowDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            SetPivot(ui, new Vector2(0.5f, 0.5f));
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HideOrder(RectTransform ui, OrderDirection direction = OrderDirection.Right, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();
        cg.blocksRaycasts = false;

        bool isVertical = (direction == OrderDirection.Top || direction == OrderDirection.Bottom);

        Sequence seq = DOTween.Sequence();

        Vector2 startPivot = new Vector2(0.5f, 0.5f);
        switch (direction)
        {
            case OrderDirection.Top: startPivot = new Vector2(0.5f, 1f); break;
            case OrderDirection.Bottom: startPivot = new Vector2(0.5f, 0f); break;
            case OrderDirection.Left: startPivot = new Vector2(0f, 0.5f); break;
            case OrderDirection.Right: startPivot = new Vector2(1f, 0.5f); break;
        }

        // 시작할 때
        seq.OnStart(() => {
            SetPivot(ui, startPivot);
            onStart?.Invoke();
        });

        if (isVertical)
        {
            seq.Append(ui.DOScaleY(0f, this.orderHideDuration).SetEase(this.orderHideEase));
        }
        else
        {
            seq.Append(ui.DOScaleX(0f, this.orderHideDuration).SetEase(this.orderHideEase));
        }

        seq.Join(cg.DOFade(0f, this.orderHideDuration).SetEase(Ease.Linear));

        seq.OnComplete(() => {
            SetPivot(ui, new Vector2(0.5f, 0.5f));
            onComplete?.Invoke();
        });
    }
    public void ShowVerticalUnfold(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        ui.localScale = new Vector3(1f, 0f, 1f);
        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScaleY(1f, this.vUnfoldShowDuration).SetEase(this.vUnfoldShowEase));
        seq.Join(cg.DOFade(1f, this.vUnfoldShowDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HideVerticalUnfold(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScaleY(0f, this.vUnfoldHideDuration).SetEase(this.vUnfoldHideEase));
        seq.Join(cg.DOFade(0f, this.vUnfoldHideDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            onComplete?.Invoke();
        });
    }
    public void ShowHorizontalUnfold(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();
        cg.DOKill();

        cg.blocksRaycasts = false;

        ui.localScale = new Vector3(0f, 1f, 1f);
        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScaleX(1f, this.hUnfoldShowDuration).SetEase(this.hUnfoldShowEase));
        seq.Join(cg.DOFade(1f, this.hUnfoldShowDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HideHorizontalUnfold(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScaleX(0f, this.hUnfoldHideDuration).SetEase(this.hUnfoldHideEase));
        seq.Join(cg.DOFade(0f, this.hUnfoldHideDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            onComplete?.Invoke();
        });
    }
    public void ShowPaper(RectTransform ui, PaperDirection direction = PaperDirection.TopRight, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();
        cg.blocksRaycasts = false;

        ui.localScale = Vector3.zero;
        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        Vector2 startPivot = GetPivotFromDirection(direction);

        // 시작할 때
        seq.OnStart(() => {
            SetPivot(ui, startPivot); 
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.one, this.paperShowDuration).SetEase(this.paperShowEase));
        seq.Join(cg.DOFade(1f, this.paperShowDuration).SetEase(Ease.Linear));

        seq.OnComplete(() => {
            SetPivot(ui, new Vector2(0.5f, 0.5f)); 
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HidePaper(RectTransform ui, PaperDirection direction = PaperDirection.TopRight, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();
        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();

        Vector2 startPivot = GetPivotFromDirection(direction);

        // 시작할 때
        seq.OnStart(() => {
            SetPivot(ui, startPivot);
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.zero, this.paperHideDuration).SetEase(this.paperHideEase));
        seq.Join(cg.DOFade(0f, this.paperHideDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            SetPivot(ui, new Vector2(0.5f, 0.5f)); 
            onComplete?.Invoke();
        });
    }
    public void ShowBounce(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Vector2 targetPos = ui.anchoredPosition;

        ui.anchoredPosition = new Vector2(targetPos.x, targetPos.y + this.bouncePopStartOffset);

        ui.localScale = Vector3.one;

        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOAnchorPos(targetPos, this.bouncePopShowDuration).SetEase(this.bouncePopShowEase));
        seq.Join(cg.DOFade(1f, this.bouncePopShowDuration * 0.5f).SetEase(Ease.Linear));

        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HideBounce(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Vector2 originalPos = ui.anchoredPosition;

        Vector2 endPos = new Vector2(ui.anchoredPosition.x, ui.anchoredPosition.y + this.zigzagDropHeight);

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOAnchorPosY(endPos.y, this.zigzagHideDuration).SetEase(this.bounceHideEase));
        seq.Join(cg.DOFade(0f, this.zigzagHideDuration).SetEase(Ease.Linear));

        // 끝날 때
        seq.OnComplete(() => {
            ui.anchoredPosition = originalPos;
            onComplete?.Invoke();
        });
    }
    public void ShowZigzag(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Vector2 targetPos = ui.anchoredPosition;

        ui.anchoredPosition = new Vector2(targetPos.x - this.zigzagLeftOffset, targetPos.y + this.zigzagDropHeight);
        ui.localScale = Vector3.one;
        cg.alpha = 0f;

        ui.localEulerAngles = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        float dropTime = this.zigzagShowDuration * 0.25f;
        float bounceTime1 = this.zigzagShowDuration * 0.35f;
        float bounceTime2 = this.zigzagShowDuration * 0.20f;
        float settleTime = this.zigzagShowDuration * 0.20f;

        seq.Join(cg.DOFade(1f, dropTime).SetEase(Ease.Linear));


        seq.Append(ui.DOAnchorPos(new Vector2(targetPos.x - this.zigzagLeftOffset, targetPos.y),
                                    dropTime).SetEase(Ease.InQuad));
        seq.Join(ui.DOLocalRotate(new Vector3(0, 0, 15f), dropTime).SetEase(Ease.InOutQuad));

        float jumpHeight1 = this.zigzagDropHeight * 0.25f;
        seq.Append(ui.DOJumpAnchorPos(new Vector2(targetPos.x + this.zigzagRightOffset, targetPos.y),
                                        jumpHeight1, 1, bounceTime1).SetEase(Ease.Linear));
        seq.Join(ui.DOLocalRotate(new Vector3(0, 0, -15f), bounceTime1).SetEase(Ease.InOutQuad));

        float jumpHeight2 = this.zigzagDropHeight * 0.08f;
        seq.Append(ui.DOJumpAnchorPos(targetPos, jumpHeight2, 1, bounceTime2).SetEase(Ease.Linear));

        seq.Join(ui.DOLocalRotate(new Vector3(0, 0, 15f), bounceTime2).SetEase(Ease.InOutQuad));

        seq.Append(ui.DOLocalRotate(Vector3.zero, settleTime).SetEase(Ease.OutQuad));

        // 끝날 때
        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    public void HideZigzag(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();

        cg.blocksRaycasts = false;

        Sequence seq = DOTween.Sequence();
        // 시작할 때
        seq.OnStart(() =>
        {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.one * this.bouncePopStartOffset, this.zigzagHideDuration).SetEase(this.zigzagHideEase));
        seq.Join(cg.DOFade(0f, this.zigzagHideDuration).SetEase(Ease.Linear));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    public void ShowStamp(RectTransform ui, UnityAction onStart = null, UnityAction onComplete = null)
    {
        if (ui == null) return;

        ui.DOKill();

        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.gameObject.AddComponent<CanvasGroup>();

        cg.DOKill();
        cg.blocksRaycasts = false;

        ui.localScale = new Vector3(this.stampStartScale.x, this.stampStartScale.y, 1f);
        cg.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // 시작할 때
        seq.OnStart(() => {
            onStart?.Invoke();
        });

        seq.Append(ui.DOScale(Vector3.one, this.stampAnimDuration).SetEase(this.stampEase));
        seq.Join(cg.DOFade(1f, this.stampAnimDuration * 0.5f).SetEase(Ease.Linear)); 

        // 끝날 때
        seq.OnComplete(() => {
            cg.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }
    private void SetPivot(RectTransform rt, Vector2 newPivot)
    {
        if (rt == null) return;

        Vector2 size = rt.rect.size;
        Vector2 deltaPivot = rt.pivot - newPivot;

        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);

        rt.pivot = newPivot;
        rt.localPosition -= deltaPosition;
    }
    private Vector2 GetPivotFromDirection(PaperDirection direction)
    {
        switch (direction)
        {
            case PaperDirection.TopLeft: return new Vector2(0f, 1f);
            case PaperDirection.TopRight: return new Vector2(1f, 1f);
            case PaperDirection.BottomLeft: return new Vector2(0f, 0f);
            case PaperDirection.BottomRight: return new Vector2(1f, 0f);
            default: return new Vector2(0.5f, 0.5f); // 기본값 중앙
        }
    }
}
