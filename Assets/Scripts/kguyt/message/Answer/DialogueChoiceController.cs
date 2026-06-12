using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("VFX")]
    [SerializeField] private AK_VFX_Manager vfxManager;

    [Header("動畫")]
    [SerializeField] private Animator panelAnimator;          // 控制整個視窗的 Animator

    [Header("參數")]
    [SerializeField] private float typewriterInterval = 0.04f; // 逐字速度（秒/字）
    [SerializeField] private float fadeOutDuration = 0.4f;  // Option 淡出時長

    // ── 私有狀態 ────────────────────────────────────────────────────
    private Coroutine activeCoroutine;
    private bool waitingForChoice;

    // ── Animator 參數名稱（需與 Animator 視窗一致） ─────────────────
    private static readonly int ANIM_LOAD_IN = Animator.StringToHash("LoadIn");
    private static readonly int ANIM_DEACTIVATE = Animator.StringToHash("Deactivate");

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

        if (panelAnimator) panelAnimator.SetTrigger(ANIM_DEACTIVATE);
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

        // 4) 播放 LoadIn 動畫讓 Options 出現（CanvasGroup 0→1 由 AnimationClip 驅動）

        

        if (!string.IsNullOrEmpty(optionAText))
        {
            SetOptionVisible(optionAGroup, true);
            optionAGroup.alpha = 0f;   // 讓 Animator clip 從 0 開始推到 1
        }
        if (!string.IsNullOrEmpty(optionBText))
        {
            SetOptionVisible(optionBGroup, true);
            optionBGroup.alpha = 0f;
        }

        if (panelAnimator != null) panelAnimator.enabled = true;
        if (panelAnimator) panelAnimator.SetTrigger(ANIM_LOAD_IN);

        // 等待動畫播完（用 AnimationClip 長度或固定等待）
        yield return new WaitForSeconds(GetAnimationLength("LoadIn"));

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

    private IEnumerator ResolveChoice(
        CanvasGroup chosenGroup, CanvasGroup otherGroup,
        DraggableOption chosenDrag, DraggableOption otherDrag)
    {
        if (panelAnimator != null) panelAnimator.enabled = false;

        // 被選中的 → 先彈回原位，再淡出到 0
        chosenDrag?.ResetPosition();
        yield return StartCoroutine(FadeOut(chosenGroup, fadeOutDuration));

        // 另一個 → 先彈回原位，再 SpawnSlashVFX，再淡出到 0
        otherDrag?.ResetPosition();

        if (vfxManager != null && otherDrag != null)
        {
            Vector2 worldPos = GetWorldPosition(otherDrag.transform as RectTransform);
            vfxManager.SpawnSlashVFX(worldPos);
        }

        yield return new WaitForSeconds(0.15f);   // 稍等讓 VFX 顯現
        yield return StartCoroutine(FadeOut(otherGroup, fadeOutDuration));
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

    /// <summary>將 RectTransform 的中心轉換為世界座標（用於 VFX Spawn）</summary>
    private static Vector2 GetWorldPosition(RectTransform rt)
    {
        if (rt == null) return Vector2.zero;
        var corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        // 四個角的平均 = 中心
        return (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
    }

    #endregion
}