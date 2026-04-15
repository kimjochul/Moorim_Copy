using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
    public int slotIndex; // 0: 왼쪽, 1: 중앙, 2: 오른쪽

    [SerializeField] private SpriteRenderer renderer;

    [Header("감정 아이콘")]
    [SerializeField] private Image emotionIcon;
    
    private int currentCharacterID = -1;
    public int CurrentCharacterID => currentCharacterID;

    private Coroutine iconCoroutine;
    
    public void SetCharacter(int characterID, Sprite sprite)
    {
        currentCharacterID = characterID;
        renderer.sprite = sprite;
        gameObject.SetActive(true);
        
        HideEmotionIcon();
    }

    public void Clear()
    {
        currentCharacterID = -1;
        renderer.sprite = null;
        
        HideEmotionIcon();
    }
    
    public void Appear(int characterID, Sprite sprite)
    {
        SetCharacter(characterID, sprite);

        StopAllCoroutines();
        StartCoroutine(AppearRoutine());
    }

    public void Disappear()
    {
        Clear();
        
        StopAllCoroutines();
        StartCoroutine(DisappearRoutine());
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
        Vector3 origin = transform.localPosition;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-power, power);
            float y = Random.Range(-power, power);

            transform.localPosition = origin + new Vector3(x, y, 0);

            yield return null;
        }

        transform.localPosition = origin;
    }
    
    public void HideEmotionIcon()
    {
        emotionIcon.gameObject.SetActive(false);
    }

    public void SetColor(Color color)
    {
        renderer.color = color;
    }
    
    IEnumerator AppearRoutine()
    {
        Vector3 origin = transform.localPosition;
        Vector3 start = origin + Vector3.left * 200f; // 왼쪽에서 등장

        transform.localPosition = start;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            transform.localPosition = Vector3.Lerp(start, origin, t);
            yield return null;
        }

        transform.localPosition = origin;
    }
    
    IEnumerator DisappearRoutine()
    {
        Vector3 origin = transform.localPosition;
        Vector3 end = origin + Vector3.right * 200f;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            transform.localPosition = Vector3.Lerp(origin, end, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
    
    public void ClearImmediate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        currentCharacterID = -1;
        renderer.sprite = null;

        HideEmotionIcon();
    }
}
