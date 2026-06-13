using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

/// <summary>
/// 掛在每個角色視窗（CharacterPanel）根物件上。
/// 子物件結構：
///   CharacterPanel
///     ├─ dialogue (TMP_Text)
///     ├─ optionA  (CanvasGroup + DraggableOption)
///     └─ optionB  (CanvasGroup + DraggableOption)
/// </summary>
public class DialogueChoiceController : MonoBehaviour
{
    // ── Inspector 綁定 ──────────────────────────────────────────────
    [Header("子物件參考")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup optionAGroup;
    [SerializeField] private CanvasGroup optionBGroup;
    [SerializeField] private DraggableOption optionADrag;
    [SerializeField] private DraggableOption optionBDrag;

    [Header("動畫")]
    [SerializeField] private Animator panelAnimator;          // 控制整個視窗的 Animator

    [Header("參數")]
    [SerializeField] private float typewriterInterval = 0.04f; // 逐字速度（秒/字）
    [SerializeField] private float fadeOutDuration = 0.4f;  // Option 淡出時長（保留給其他用途，掉落本身不淡出）

    [Header("掉落效果參數")]
    [SerializeField] private float dropFallDistance = 1200f;  // 掉出畫面的距離（px）
    [SerializeField] private float dropDuration = 0.6f;       // 掉落動畫時長
    [SerializeField] private float dropRotationSpeed = 180f;  // 掉落時每秒旋轉角度

    private Action onResolveComplete;
    public void SetOnResolveComplete(Action callback) => onResolveComplete = callback;

    // ── 私有狀態 ────────────────────────────────────────────────────
    private Coroutine activeCoroutine;
    private bool waitingForChoice;

    // ── Animator 參數名稱（需與 Animator 視窗一致） ─────────────────
    private static readonly int ANIM_LOAD_IN = Animator.StringToHash("LoadIn");

    // ═══════════════════════════════════════════════════════════════
    #region Unity Lifecycle

    private void Start()
    {
        // 遊戲開始時隱藏兩個 option，等 PlayDialogue 呼叫後才顯示
        HideOptionsImmediate();
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 公開 API（供 CharacterViewSwitcher 呼叫）

    /// <summary>由 CharacterViewSwitcher 在切換完成後呼叫，啟動對話流程</summary>
    public void Activate() { }   // 切換後由 PlayDialogue 驅動，這裡保留給未來擴充

    /// <summary>離開此視窗前清理狀態</summary>
    public void Deactivate()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        waitingForChoice = false;

        // 隱藏 Options、清空文字
        SetOptionVisible(optionAGroup, false);
        SetOptionVisible(optionBGroup, false);
        dialogueText.text = string.Empty;

        // 通知 DraggableOption 停止監聽
        optionADrag?.SetInteractable(false);
        optionBDrag?.SetInteractable(false);

    }

    /// <summary>
    /// 主要對話入口。
    /// <param name="text">要顯示的對話內容</param>
    /// <param name="optionAText">選項 A 文字（可為 null 表示不顯示）</param>
    /// <param name="optionBText">選項 B 文字（可為 null 表示不顯示）</param>
    /// </summary>
    public void PlayDialogue(string text, string optionAText = null, string optionBText = null)
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(DialogueSequence(text, optionAText, optionBText));
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 核心流程

    private IEnumerator DialogueSequence(string text, string optionAText, string optionBText)
    {
        // 1) 先把 Options 隱藏、禁用互動、位置歸零
        SetOptionVisible(optionAGroup, false);
        SetOptionVisible(optionBGroup, false);
        optionADrag?.SetInteractable(false);
        optionBDrag?.SetInteractable(false);
        optionADrag?.ResetPosition();   // ← 確保每次重新播放時位置歸零
        optionBDrag?.ResetPosition();

        // 2) 逐字顯示對話文字
        yield return StartCoroutine(TypewriterRoutine(text));

        // 3) 設定 Option 文字（若有）
        SetOptionLabel(optionADrag, optionAText);
        SetOptionLabel(optionBDrag, optionBText);

        // 4) 直接顯示 Options（不淡入）
        if (!string.IsNullOrEmpty(optionAText))
        {
            optionAGroup.alpha = 1f;
            optionAGroup.blocksRaycasts = true;
            optionAGroup.interactable = true;
        }
        if (!string.IsNullOrEmpty(optionBText))
        {
            optionBGroup.alpha = 1f;
            optionBGroup.blocksRaycasts = true;
            optionBGroup.interactable = true;
        }

        // 通知視窗 Animator 播 LoadIn（視窗本身的進場動畫，不控制 option alpha）
        if (panelAnimator != null && panelAnimator.enabled)
            panelAnimator.SetTrigger(ANIM_LOAD_IN);

        // 5) 啟用拖曳互動
        bool hasOptions = !string.IsNullOrEmpty(optionAText) || !string.IsNullOrEmpty(optionBText);
        if (hasOptions)
        {
            waitingForChoice = true;

            if (!string.IsNullOrEmpty(optionAText)) optionADrag?.SetInteractable(true);
            if (!string.IsNullOrEmpty(optionBText)) optionBDrag?.SetInteractable(true);

            // 設定回呼
            optionADrag?.SetOnDropped(() => OnOptionDropped(optionAGroup, optionBGroup, optionADrag, optionBDrag));
            optionBDrag?.SetOnDropped(() => OnOptionDropped(optionBGroup, optionAGroup, optionBDrag, optionADrag));

            // 等待玩家選擇（waitingForChoice 由 OnOptionDropped 設為 false）
            yield return new WaitUntil(() => !waitingForChoice);
        }

        activeCoroutine = null;
    }

    [Header("視覺回饋")]
    [SerializeField] private MaskFlashController maskFlashController;

    // ─────────────────────────────────────────────────────────────
    /// <summary>玩家放開 option 後的處理</summary>
    /// <param name="chosen">被選中的那個 CanvasGroup</param>
    /// <param name="other">另一個 CanvasGroup</param>
    private void OnOptionDropped(
        CanvasGroup chosenGroup, CanvasGroup otherGroup,
        DraggableOption chosenDrag, DraggableOption otherDrag)
    {
        if (!waitingForChoice) return;   // 防止雙重觸發
        waitingForChoice = false;

        // 停用兩個互動
        chosenDrag?.SetInteractable(false);
        otherDrag?.SetInteractable(false);

        StartCoroutine(ResolveChoice(chosenGroup, otherGroup, chosenDrag, otherDrag));
    }

    /// <summary>
    /// 玩家選擇後的處理：
    /// 1) 先設定本次選項對應的 Image 給 MaskFlashController（供外部腳本判定後呼叫 PlayCorrectFlash/PlayWrongFlash）
    /// 2) 被選中的 → 淡出變透明
    /// 3) 另一個 → 掉出畫面外
    /// </summary>
    private IEnumerator ResolveChoice(
        CanvasGroup chosenGroup, CanvasGroup otherGroup,
        DraggableOption chosenDrag, DraggableOption otherDrag)
    {
        // 設定本次要變色的兩個 Image，供外部判定腳本呼叫 PlayCorrectFlash() / PlayWrongFlash()
        Image chosenImage = chosenDrag != null ? chosenDrag.GetComponent<Image>() : null;
        Image otherImage = otherDrag != null ? otherDrag.GetComponent<Image>() : null;
        maskFlashController?.SetOptionImages(chosenImage, otherImage);

        // 1) 被選中的 → 淡出變透明
        if (chosenDrag != null)
        {
            yield return StartCoroutine(FadeOut(chosenGroup, fadeOutDuration));
            SetOptionVisible(chosenGroup, false);
            chosenDrag.ResetPosition();
        }

        // 2) 另一個 → 掉出畫面外
        if (otherDrag != null)
        {
            yield return StartCoroutine(DropOutRoutine(otherDrag.transform as RectTransform));
            SetOptionVisible(otherGroup, false);
            otherDrag.ResetPosition();
        }

        onResolveComplete?.Invoke();
        onResolveComplete = null;
    }

    /// <summary>淡出指定 CanvasGroup 至透明</summary>
    private IEnumerator FadeOut(CanvasGroup group, float duration)
    {
        if (group == null) yield break;

        float start = group.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, 0f, elapsed / duration);
            yield return null;
        }
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    /// <summary>
    /// 兩個選項同時掉出畫面（例如：強制事件、無法選擇的結局）
    /// 呼叫此方法後，兩個選項立即禁用拖曳互動。
    /// </summary>
    public void ResolveChoiceBoth()
    {
        if (!waitingForChoice) return; // 防止重複觸發
        waitingForChoice = false;

        // 立即停用兩個選項的拖曳互動
        optionADrag?.SetInteractable(false);
        optionBDrag?.SetInteractable(false);

        StartCoroutine(ResolveBothDrop());
    }

    private IEnumerator ResolveBothDrop()
    {
        Coroutine dropA = null, dropB = null;

        if (optionADrag != null)
            dropA = StartCoroutine(DropOutRoutine(optionADrag.transform as RectTransform));

        if (optionBDrag != null)
            dropB = StartCoroutine(DropOutRoutine(optionBDrag.transform as RectTransform));

        if (dropA != null) yield return dropA;
        if (dropB != null) yield return dropB;

        SetOptionVisible(optionAGroup, false);
        SetOptionVisible(optionBGroup, false);
        optionADrag?.ResetPosition();
        optionBDrag?.ResetPosition();

        onResolveComplete?.Invoke();
        onResolveComplete = null;
    }


   
    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 輔助方法

    private IEnumerator TypewriterRoutine(string text)
    {
        dialogueText.text = string.Empty;
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterInterval);
        }
    }

    private static void SetOptionVisible(CanvasGroup group, bool visible)
    {
        if (group == null) return;
        group.alpha = visible ? 1f : 0f;
        group.blocksRaycasts = visible;
        group.interactable = visible;
    }

    /// <summary>
    /// 遊戲開始時用：保持 GameObject Active（讓 DraggableOption.Awake 能記錄原始位置），
    /// 但把 alpha 設為 0 且禁用 raycast，視覺上不可見且無法互動。
    /// </summary>
    private void HideOptionsImmediate()
    {
        if (optionAGroup != null)
        {
            optionAGroup.alpha = 0f;
            optionAGroup.blocksRaycasts = false;
        }
        if (optionBGroup != null)
        {
            optionBGroup.alpha = 0f;
            optionBGroup.blocksRaycasts = false;
        }
    }

    private static void SetOptionLabel(DraggableOption drag, string label)
    {
        if (drag == null || label == null) return;
        var tmp = drag.GetComponentInChildren<TMP_Text>();
        if (tmp) tmp.text = label;
    }

    /// <summary>取得 Animator 中指定 clip 的長度，找不到時回傳 0.5f</summary>
    private float GetAnimationLength(string clipName)
    {
        if (panelAnimator == null) return 0.5f;
        foreach (var clip in panelAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }
        return 0.5f;
    }

    /// <summary>
    /// 讓 UI RectTransform 直接往下掉出畫面外（加速下墜 + 旋轉），不需要 Rigidbody2D。
    /// 結束後不會自動歸位，由呼叫端決定何時 ResetPosition()。
    /// </summary>
    /// <param name="rt">要播放動畫的 RectTransform</param>
    private IEnumerator DropOutRoutine(RectTransform rt)
    {
        if (rt == null) yield break;

        Vector2 startPos = rt.anchoredPosition;
        float startRotZ = rt.localEulerAngles.z;

        // 隨機決定旋轉方向，讓掉落更自然
        float rotDir = UnityEngine.Random.value > 0.5f ? 1f : -1f;

        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropDuration;

            // 加速下墜（ease-in）
            float yOffset = -dropFallDistance * (t * t);
            rt.anchoredPosition = startPos + new Vector2(0f, yOffset);

            // 翻滾旋轉
            rt.localRotation = Quaternion.Euler(0f, 0f, startRotZ + rotDir * dropRotationSpeed * elapsed);

            yield return null;
        }
    }

    #endregion
}