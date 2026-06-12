using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueChoiceController : MonoBehaviour
{
    [Header("逐字顯示")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private string defaultText = "預設測試對話文字...";

    [Header("選項動畫母物件")]
    [SerializeField] private Animator choiceAnimator;

    [Header("選項物件")]
    [SerializeField] private RectTransform choiceA;
    [SerializeField] private RectTransform choiceB;

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

    // 選項原始狀態
    private Vector2 choiceAOriginPos;
    private Vector2 choiceBOriginPos;

    // 測試用 InputAction
    private InputAction testTypeAction;
    private InputAction testChooseAAction;
    private InputAction testChooseBAction;
    private InputAction testBothSlashAction;

    // ────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && !rootCanvas.isRootCanvas)
            rootCanvas = rootCanvas.rootCanvas;

        // 記錄選項原始位置
        if (choiceA != null) choiceAOriginPos = choiceA.anchoredPosition;
        if (choiceB != null) choiceBOriginPos = choiceB.anchoredPosition;

        testTypeAction      = new InputAction("TestType",      binding: "<Keyboard>/t");
        testChooseAAction   = new InputAction("TestChooseA",   binding: "<Keyboard>/z");
        testChooseBAction   = new InputAction("TestChooseB",   binding: "<Keyboard>/x");
        testBothSlashAction = new InputAction("TestBothSlash", binding: "<Keyboard>/c");
    }

    private void OnEnable()
    {
        testTypeAction.performed      += _ => { if (isActive) ShowText(defaultText); };
        testChooseAAction.performed   += _ => { if (isActive) SimulateDrop(choiceA, choiceB); };
        testChooseBAction.performed   += _ => { if (isActive) SimulateDrop(choiceB, choiceA); };
        testBothSlashAction.performed += _ => { if (isActive) TriggerBothSlash(); };

        testTypeAction.Enable();
        testChooseAAction.Enable();
        testChooseBAction.Enable();
        testBothSlashAction.Enable();
    }

    private void OnDisable()
    {
        testTypeAction.Disable();
        testChooseAAction.Disable();
        testChooseBAction.Disable();
        testBothSlashAction.Disable();
    }

    private void Update()
    {
        if (isActive) HandleDrag();
    }

    #endregion

    // ────────────────────────────────────────
    #region 公開控制介面

    /// <summary>
    /// 由 CharacterViewSwitcher 呼叫，啟用此 Controller 並開始顯示文字
    /// </summary>
    public void Activate()
    {
        isActive = true;
        ResetChoices();
        ShowText(defaultText);
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
    /// 選項回到原始狀態
    /// </summary>
    public void ResetChoices()
    {
        ResetChoice(choiceA, choiceAOriginPos);
        ResetChoice(choiceB, choiceBOriginPos);
    }

    private void ResetChoice(RectTransform rect, Vector2 originPos)
    {
        if (rect == null) return;

        rect.anchoredPosition = originPos;

        // 確保 alpha 為 0，等 LoadIn 動畫來控制顯示
        CanvasGroup cg = rect.GetComponent<CanvasGroup>();
        if (cg == null) cg = rect.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        rect.gameObject.SetActive(true);
    }

    #endregion

    // ────────────────────────────────────────
    #region ① 逐字顯示

    public void ShowText(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeRoutine(text));
    }

    private IEnumerator TypeRoutine(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        OnTypingComplete();
    }

    private void OnTypingComplete()
    {
        choiceAnimator.SetTrigger("LoadIn");
    }

    #endregion

    // ────────────────────────────────────────
    #region ③ 拖曳邏輯

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
                    rootCanvas.GetComponent<RectTransform>(),
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

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    rect, mouseScreenPos, GetCanvasCamera()))
            {
                draggingRect = rect;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootCanvas.GetComponent<RectTransform>(),
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
    #region ④ 特效 & 淡出

    private void TriggerSlashOn(RectTransform target)
    {
        if (target == null) return;
        vfxManager.SpawnSlashVFX(target.position);
    }

    public void TriggerBothSlash()
    {
        TriggerSlashOn(choiceA);
        TriggerSlashOn(choiceB);
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
        // SetActive(false) 移除，alpha 0 即不可見，Reset 時再統一處理
    }

    #endregion

    // ────────────────────────────────────────
    #region 測試輔助

    private void SimulateDrop(RectTransform dragged, RectTransform other)
    {
        OnChoiceDropped(dragged, other);
    }

    #endregion
}