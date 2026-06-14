using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ¨₪¦ג¥ßֳ¸°Êµe±±¨מ¾¹¡C
/// µ×¹ן®ֹ¼u¸ץ¡Bµ×¿ש®ֹ¾_°Ê¡Aµ²§פ«ב¦^¨ל­ל¦ל¡C
/// ±¾¦b¥פ·N GameObject ₪W¡A±N¥ßֳ¸×÷ Image ©ל₪J characterImage ִז¦ל¡C
/// </summary>
public class CharacterAnimController : MonoBehaviour
{
    [Header("¥ßֳ¸°ׁ¦ׂ")]
    [SerializeField] private Image characterImage;

    [Header("¼u¸ץ³]©w¡]µ×¹ן¡^")]
    [SerializeField] private float bounceHeight = 30f;      // ¸ץ°_°×«׳¡]px¡^
    [SerializeField] private float bounceUpDuration = 0.15f;
    [SerializeField] private float bounceDownDuration = 0.2f;
    [SerializeField] private int bounceTimes = 2;         // ¼u¸ץ¦¸¼ֶ

    [Header("¾_°Ê³]©w¡]µ×¿ש¡^")]
    [SerializeField] private float shakeDistance = 12f;     // ¥×¥k¦ל²¾¡]px¡^
    [SerializeField] private float shakeDuration = 0.08f;   // ¨C¦¸¥×¥k®ֹ×ר
    [SerializeField] private int shakeTimes = 4;          // ¥×¥k¨׃¦^¦¸¼ֶ

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Coroutine activeAnim;

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region Unity

    private void Awake()
    {
        if (characterImage != null)
            rectTransform = characterImage.rectTransform;

        if (rectTransform != null)
            originalPosition = rectTransform.anchoredPosition;
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region ₪½¶} API

    public void PlayCorrectAnim()
    {
        if (rectTransform == null) return;
        StopActive();
        activeAnim = StartCoroutine(BounceRoutine());
    }

    public void PlayWrongAnim()
    {
        if (rectTransform == null) return;
        StopActive();
        activeAnim = StartCoroutine(ShakeRoutine());
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region °Êµe Coroutine

    private IEnumerator BounceRoutine()
    {
        for (int i = 0; i < bounceTimes; i++)
        {
            // ©¹₪W
            yield return MoveY(originalPosition.y, originalPosition.y + bounceHeight, bounceUpDuration, easeOut: true);
            // ©¹₪U
            yield return MoveY(originalPosition.y + bounceHeight, originalPosition.y, bounceDownDuration, easeIn: true);
        }
        SetY(originalPosition.y);
        activeAnim = null;
    }

    private IEnumerator ShakeRoutine()
    {
        for (int i = 0; i < shakeTimes; i++)
        {
            // ¥k
            yield return MoveX(originalPosition.x, originalPosition.x + shakeDistance, shakeDuration);
            // ¥×
            yield return MoveX(originalPosition.x + shakeDistance, originalPosition.x - shakeDistance, shakeDuration);
        }
        // ֲk¦ל
        yield return MoveX(originalPosition.x - shakeDistance, originalPosition.x, shakeDuration * 0.5f);
        SetX(originalPosition.x);
        activeAnim = null;
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region »²§U

    private IEnumerator MoveY(float from, float to, float duration, bool easeOut = false, bool easeIn = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = ApplyEase(t, easeIn, easeOut);
            SetY(Mathf.Lerp(from, to, t));
            yield return null;
        }
        SetY(to);
    }

    private IEnumerator MoveX(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetX(Mathf.Lerp(from, to, t));
            yield return null;
        }
        SetX(to);
    }

    private float ApplyEase(float t, bool easeIn, bool easeOut)
    {
        if (easeIn && easeOut) return t * t * (3f - 2f * t); // smoothstep
        if (easeIn) return t * t;
        if (easeOut) return 1f - (1f - t) * (1f - t);
        return t;
    }

    private void SetY(float y)
    {
        if (rectTransform == null) return;
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = y;
        rectTransform.anchoredPosition = pos;
    }

    private void SetX(float x)
    {
        if (rectTransform == null) return;
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x = x;
        rectTransform.anchoredPosition = pos;
    }

    private void StopActive()
    {
        if (activeAnim != null)
        {
            StopCoroutine(activeAnim);
            activeAnim = null;
        }
        // ±j¨מֲk¦ל¡Aֱ׳§K°Êµe³Q¥´ֲ_«ב¦ל¸m´Ý¯d
        if (rectTransform != null)
            rectTransform.anchoredPosition = originalPosition;
    }

    #endregion
}