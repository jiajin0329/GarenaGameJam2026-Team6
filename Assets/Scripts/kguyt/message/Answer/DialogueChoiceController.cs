using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class DialogueChoiceController : MonoBehaviour
{
    [Header("逐字顯示")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("選項動畫母物件")]
    [SerializeField] private Animator choiceAnimator;

    [Header("選項物件")]
    [SerializeField] private RectTransform choiceA;
    [SerializeField] private RectTransform choiceB;

    [Header("選項文字")]
    [SerializeField] private TMP_Text choiceAText;
    [SerializeField] private TMP_Text choiceBText;

    [Header("特效管理")]
    [SerializeField] private AK_VFX_Manager vfxManager;

    [Header("淡出設定")]
    [SerializeField] private float fadeDuration = 0.5f;

    // Canvas 參考
    private Canvas rootCanvas;
    private Coroutine typingCoroutine;

    // 拖曳狀態
    private RectTransform draggingRect = null;
    private Vector2 dragOffset;

    // 是否啟用輸入（由 CharacterViewSwitcher 控制）
    private bool isActive = false;

    // 已放開的選項（不可再拖）
    private readonly HashSet<RectTransform> droppedChoices = new HashSet<RectTransform>();

    // 選項原始位置
    private Vector2 choiceAOriginPos;
    private Vector2 choiceBOriginPos;

    // ────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && !rootCanvas.isRootCanvas)
            rootCanvas = rootCanvas.rootCanvas;

        if (choiceA != null) choiceAOriginPos = choiceA.anchoredPosition;
        if (choiceB != null) choiceBOriginPos = choiceB.anchoredPosition;
    }

    private void Update()
    {
        if (isActive) HandleDrag();
    }

    #endregion

    // ────────────────────────────────────────
    #region 公開接口

    /// <summary>
    /// 設定問題文字並開始逐字顯示，顯示完畢後自動觸發 LoadIn 動畫
    /// </summary>
    public void SetQuestionText(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeRoutine(text));
    }

    /// <summary>
    /// 設定選項A的顯示文字
    /// </summary>
    public void SetSelection1Text(string text)
    {
        if (choiceAText != null)
            choiceAText.text = text;
    }

    /// <summary>
    /// 設定選項B的顯示文字
    /// </summary>
    public void SetSelection2Text(string text)
    {
        if (choiceBText != null)
            choiceBText.text = text;
    }

    /// <summary>
    /// 由 CharacterViewSwitcher 呼叫，啟用此 Controller
    /// </summary>
    public void Activate()
    {
        isActive = true;
        ResetChoices();
    }

    /// <summary>
    /// 由 CharacterViewSwitcher 呼叫，停用此 Controller
    /// </summary>
    public void Deactivate()
    {
        isActive = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        draggingRect = null;
    }

    /// <summary>
    /// 兩個選項同時播放 SlashVFX
    /// </summary>
    public void TriggerBothSlash()
    {
        TriggerSlashOn(choiceA);
        TriggerSlashOn(choiceB);
    }

    /// <summary>
    /// 重置選項狀態（回原位、alpha=0、清除拖曳記錄）
    /// </summary>
    public void ResetChoices()
    {
        droppedChoices.Clear();
        ResetChoice(choiceA, choiceAOriginPos);
        ResetChoice(choiceB, choiceBOriginPos);
    }

    #endregion

    // ────────────────────────────────────────
    #region 逐字顯示

    private IEnumerator TypeRoutine(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        choiceAnimator.SetTrigger("LoadIn");
    }

    #endregion

    // ────────────────────────────────────────
    #region 拖曳邏輯

    private void HandleDrag()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseScreenPos = mouse.position.value;

        if (mouse.leftButton.wasPressedThisFrame)
            TryBeginDrag(mouseScreenPos);

        if (mouse.leftButton.isPressed && draggingRect != null)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggingRect.parent as RectTransform,
                    mouseScreenPos,
                    GetCanvasCamera(),
                    out Vector2 localPoint))
            {
                draggingRect.localPosition = localPoint + dragOffset;
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame && draggingRect != null)
        {
            RectTransform other = (draggingRect == choiceA) ? choiceB : choiceA;
            OnChoiceDropped(draggingRect, other);
            draggingRect = null;
        }
    }

    private void TryBeginDrag(Vector2 mouseScreenPos)
    {
        foreach (var rect in new[] { choiceA, choiceB })
        {
            if (rect == null || !rect.gameObject.activeInHierarchy) continue;
            if (droppedChoices.Contains(rect)) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    rect, mouseScreenPos, GetCanvasCamera()))
            {
                draggingRect = rect;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    draggingRect.parent as RectTransform,
                    mouseScreenPos,
                    GetCanvasCamera(),
                    out Vector2 localPoint);

                dragOffset = (Vector2)rect.localPosition - localPoint;
                break;
            }
        }
    }

    private void OnChoiceDropped(RectTransform dragged, RectTransform other)
    {
        droppedChoices.Add(dragged);
        StartCoroutine(FadeOut(dragged));
        TriggerSlashOn(other);
    }

    private Camera GetCanvasCamera()
    {
        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;
        return rootCanvas.worldCamera != null ? rootCanvas.worldCamera : Camera.main;
    }

    #endregion

    // ────────────────────────────────────────
    #region 特效 & 淡出

    private void TriggerSlashOn(RectTransform target)
    {
        if (target == null) return;
        vfxManager.SpawnSlashVFX(target.position);
    }

    private IEnumerator FadeOut(RectTransform target)
    {
        if (target == null) yield break;

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.gameObject.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
    }

    private void ResetChoice(RectTransform rect, Vector2 originPos)
    {
        if (rect == null) return;

        rect.anchoredPosition = originPos;

        CanvasGroup cg = rect.GetComponent<CanvasGroup>();
        if (cg == null) cg = rect.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        rect.gameObject.SetActive(true);
    }

    #endregion
}