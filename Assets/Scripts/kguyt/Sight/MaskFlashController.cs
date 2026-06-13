using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 視覺遮罩閃爍控制器。
/// 用於答對/答錯時，讓畫面上的白色遮罩（含子遮罩）變色後淡出，
/// 同時讓兩個選項的圖片短暫變色提示。
/// 
/// 使用方式：
///   1. 在 ResolveChoice 中先呼叫 SetOptionImages(imgA, imgB) 設定目標
///   2. 再呼叫 PlayCorrectFlash() 或 PlayWrongFlash()（無參數）
/// </summary>
public class MaskFlashController : MonoBehaviour
{
    [Header("遮罩參考")]
    [SerializeField] private Image maskRoot;   // 外層遮罩
    [SerializeField] private Image maskChild;  // 內層子遮罩（與外層同步）

    [Header("顏色設定")]
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color optionNormalColor = Color.white;

    [Header("時間設定")]
    [SerializeField] private float maskFadeInDuration = 0.2f;  // 遮罩淡入時長（新增）
    [SerializeField] private float maskFadeDuration = 0.5f;    // 遮罩淡出時長
    [SerializeField] private float optionFlashDuration = 0.3f; // 選項變色時長
    [SerializeField] private float optionFlashHold = 0.2f;     // 選項變色後停留時間
    [SerializeField] private float optionFlashBackDuration = 0.3f; // 選項變回原色時長

    // 由外部（DialogueChoiceController）在 ResolveChoice 時設定
    private Image optionAImage;
    private Image optionBImage;

    private Coroutine activeRoutine;

    // ═══════════════════════════════════════════════════════════════
    #region 公開 API

    /// <summary>
    /// 設定本次要變色的兩個選項 Image。
    /// 由 DialogueChoiceController 在 ResolveChoice 開頭呼叫一次，
    /// 之後即可直接呼叫 PlayCorrectFlash() / PlayWrongFlash()。
    /// </summary>
    public void SetOptionImages(Image imageA, Image imageB)
    {
        optionAImage = imageA;
        optionBImage = imageB;
    }

    /// <summary>答錯時呼叫：遮罩變紅後淡出，已設定的選項圖片短暫變紅</summary>
    public void PlayWrongFlash()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(FlashRoutine(wrongColor));
    }

    /// <summary>答對時呼叫：遮罩變綠後淡出，已設定的選項圖片短暫變綠</summary>
    public void PlayCorrectFlash()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(FlashRoutine(correctColor));
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 核心流程

    private void Awake()
    {
        SetMaskAlpha(0f);
    }
    private IEnumerator FlashRoutine(Color flashColor)
    {
        // 1) 遮罩從目前 alpha 漸變到 flashColor (alpha = 1)
        Coroutine maskFadeIn = StartCoroutine(MaskFadeInRoutine(flashColor));

        // 2) 同步：選項變色
        Coroutine optionRoutine = StartCoroutine(OptionFlashRoutine(flashColor));

        yield return maskFadeIn;

        // 3) 遮罩淡出
        yield return StartCoroutine(MaskFadeOutRoutine());

        // 等選項變色流程結束（避免提早結束）
        yield return optionRoutine;

        activeRoutine = null;
    }

    /// <summary>讓遮罩從目前顏色漸變到 targetColor（alpha = 1）</summary>
    private IEnumerator MaskFadeInRoutine(Color targetColor)
    {
        Color startA = maskRoot != null ? maskRoot.color : targetColor;
        Color startB = maskChild != null ? maskChild.color : targetColor;
        Color target = targetColor;
        target.a = 1f;

        float elapsed = 0f;
        while (elapsed < maskFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / maskFadeInDuration;
            if (maskRoot != null) maskRoot.color = Color.Lerp(startA, target, t);
            if (maskChild != null) maskChild.color = Color.Lerp(startB, target, t);
            yield return null;
        }
        if (maskRoot != null) maskRoot.color = target;
        if (maskChild != null) maskChild.color = target;
    }

    /// <summary>讓遮罩 alpha 從 1 淡到 0</summary>
    private IEnumerator MaskFadeOutRoutine()
    {
        float elapsed = 0f;
        while (elapsed < maskFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / maskFadeDuration);
            SetMaskAlpha(alpha);
            yield return null;
        }
        SetMaskAlpha(0f);
    }

    /// <summary>讓兩個選項圖片變色，停留一下，再變回原色</summary>
    private IEnumerator OptionFlashRoutine(Color flashColor)
    {
        // 變色
        yield return StartCoroutine(LerpOptionColors(optionNormalColor, flashColor, optionFlashDuration));

        // 停留
        yield return new WaitForSeconds(optionFlashHold);

        // 變回原色
        yield return StartCoroutine(LerpOptionColors(flashColor, optionNormalColor, optionFlashBackDuration));
    }

    private IEnumerator LerpOptionColors(Color from, Color to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color c = Color.Lerp(from, to, elapsed / duration);
            SetOptionColor(c);
            yield return null;
        }
        SetOptionColor(to);
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 輔助方法

    
    private void SetMaskAlpha(float alpha)
    {
        if (maskRoot != null)
        {
            Color c = maskRoot.color;
            c.a = alpha;
            maskRoot.color = c;
        }
        if (maskChild != null)
        {
            Color c = maskChild.color;
            c.a = alpha;
            maskChild.color = c;
        }
    }

    /// <summary>設定目前已記錄的兩個選項圖片的顏色</summary>
    private void SetOptionColor(Color color)
    {
        if (optionAImage != null) optionAImage.color = color;
        if (optionBImage != null) optionBImage.color = color;
    }

    #endregion
}