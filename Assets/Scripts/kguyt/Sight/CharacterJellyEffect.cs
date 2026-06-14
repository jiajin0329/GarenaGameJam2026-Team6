using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// ֲIְ»¥ßֳ¸¡G÷NְY₪Uְ£¦A¼u°_®ִ×G
/// µ×¿ש¡G×G­ב¥×¥k·nֲ\
/// ±¾¦b¥ßֳ¸ Image ×÷ GameObject ₪W§Y¥i
/// </summary>
[RequireComponent(typeof(Image))]
public class CharacterJellyEffect : MonoBehaviour, IPointerDownHandler
{
    [Header("÷NְY³]©w¡]ֲIְ»¡^")]
    [SerializeField] private float pressDownY = -20f;       // ₪Uְ£¦ל²¾¡]px¡A­t¼ֶ©¹₪U¡^
    [SerializeField] private float pressDownDuration = 0.1f;
    [SerializeField] private Vector3 pressSquash = new Vector3(1.1f, 0.88f, 1f); // ₪Uְ£®ֹ»´·Lְ£«ף
    [SerializeField] private float bounceUpY = 10f;         // ¼u°_¹L½ִ°×«׳¡]px¡^
    [SerializeField] private float bounceUpDuration = 0.15f;
    [SerializeField] private float settleDuration = 0.12f;  // ¹L½ִ«בֲk¦ל

    [Header("×G­ב·nֲ\³]©w¡]µ×¿ש¡^")]
    [SerializeField] private float swingAngle = 8f;
    [SerializeField] private float swingDuration = 0.12f;
    [SerializeField] private int swingCount = 6;
    [SerializeField] private float damping = 0.65f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private float originalRotationZ;
    private Coroutine activeAnim;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = transform.localScale;
        originalRotationZ = transform.localEulerAngles.z;
    }

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region ₪½¶} API

    /// <summary>ֲIְ»¥ßֳ¸ִ²µo÷NְY₪Uְ£¼u°_</summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        StopActive();
        activeAnim = StartCoroutine(PressHeadRoutine());
    }

    /// <summary>µ×¿ש®ֹ¥ׁ¥~³¡©I¥s¡Aִ²µo×G­ב·nֲ\</summary>
    public void PlayWrongJelly()
    {
        StopActive();
        activeAnim = StartCoroutine(JellyRoutine());
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region ÷NְY¬yµ{

    private IEnumerator PressHeadRoutine()
    {
        // 1) ₪Uְ£¡GYֱY₪p¡BXֵÜ¼e
        Vector3 pressScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.82f, 1f);
        yield return LerpScale(transform.localScale, pressScale, pressDownDuration, easeOut: true);

        // 2) ¼u°_¹L½ִ¡GY©װ°×¡BXֱY¦^
        Vector3 bounceScale = new Vector3(originalScale.x * 0.93f, originalScale.y * 1.15f, 1f);
        yield return LerpScale(transform.localScale, bounceScale, bounceUpDuration, easeOut: true);

        // 3) ֲk¦ל
        yield return LerpScale(transform.localScale, originalScale, settleDuration, smoothStep: true);

        transform.localScale = originalScale;
        activeAnim = null;
    }

    private IEnumerator LerpScale(Vector3 from, Vector3 to, float duration,
                               bool easeOut = false, bool smoothStep = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (smoothStep) t = t * t * (3f - 2f * t);
            else if (easeOut) t = 1f - (1f - t) * (1f - t);
            transform.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }
        transform.localScale = to;
    }

    private IEnumerator PressDown()
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = originalPosition + new Vector2(0, pressDownY);
        Vector3 startScale = transform.localScale;

        while (elapsed < pressDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOut(Mathf.Clamp01(elapsed / pressDownDuration));
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            transform.localScale = Vector3.Lerp(startScale, pressSquash, t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
        transform.localScale = pressSquash;
    }

    private IEnumerator BounceUp()
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = originalPosition + new Vector2(0, bounceUpY);
        Vector3 startScale = transform.localScale;

        while (elapsed < bounceUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOut(Mathf.Clamp01(elapsed / bounceUpDuration));
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
        transform.localScale = originalScale;
    }

    private IEnumerator Settle()
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;
            float t = SmoothStep(Mathf.Clamp01(elapsed / settleDuration));
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region ×G­ב·nֲ\¬yµ{

    private IEnumerator JellyRoutine()
    {
        float angle = swingAngle;
        float direction = 1f;

        for (int i = 0; i < swingCount; i++)
        {
            float targetAngle = originalRotationZ + angle * direction;
            yield return LerpRotation(transform.localEulerAngles.z, targetAngle, swingDuration);
            angle *= damping;
            direction *= -1f;
        }

        yield return LerpRotation(transform.localEulerAngles.z, originalRotationZ, swingDuration * 0.8f);
        SetRotationZ(originalRotationZ);
        activeAnim = null;
    }

    private IEnumerator LerpRotation(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOut(Mathf.Clamp01(elapsed / duration));
            SetRotationZ(Mathf.LerpAngle(from, to, t));
            yield return null;
        }
        SetRotationZ(to);
    }

    #endregion

    // שששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששששש
    #region »²§U

    private void StopActive()
    {
        if (activeAnim != null)
        {
            StopCoroutine(activeAnim);
            activeAnim = null;
        }
        ResetAll();
    }

    private void ResetAll()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = originalPosition;
        transform.localScale = originalScale;
        SetRotationZ(originalRotationZ);
    }

    private void SetRotationZ(float z)
    {
        Vector3 e = transform.localEulerAngles;
        e.z = z;
        transform.localEulerAngles = e;
    }

    private float EaseOut(float t) => 1f - (1f - t) * (1f - t);
    private float SmoothStep(float t) => t * t * (3f - 2f * t);

    #endregion
}