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

    [Header("VFX")]
    [SerializeField] private AK_VFX_Manager vfxManager;

    [Header("動畫")]
    [SerializeField] private Animator panelAnimator;          // 控制整個視窗的 Animator

    [Header("參數")]
    [SerializeField] private float typewriterInterval = 0.04f; // 逐字速度（秒/字）
    [SerializeField] private float fadeOutDuration = 0.4f;  // Option 淡出時長
    [SerializeField] private float fadeInDuration = 0.3f;  // Option 淡入時長

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

        // 4) 程式碼 FadeIn 讓 Options 出現（不走 Animator，避免 enabled 重啟導致 clip 跳過）
        if (!string.IsNullOrEmpty(optionAText))
        {
            optionAGroup.alpha = 0f;
            optionAGroup.blocksRaycasts = false;
            optionAGroup.interactable = false;
        }
        if (!string.IsNullOrEmpty(optionBText))
        {
            optionBGroup.alpha = 0f;
            optionBGroup.blocksRaycasts = false;
            optionBGroup.interactable = false;
        }

        // 通知視窗 Animator 播 LoadIn（視窗本身的進場動畫，不控制 option alpha）
        if (panelAnimator != null && panelAnimator.enabled)
            panelAnimator.SetTrigger(ANIM_LOAD_IN);

        // 同步淡入兩個 option
        yield return StartCoroutine(FadeInOptions(
            !string.IsNullOrEmpty(optionAText) ? optionAGroup : null,
            !string.IsNullOrEmpty(optionBText) ? optionBGroup : null,
            fadeInDuration));

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
        // 被選中的 → 拋物線向上飛出並淡出
        if (chosenDrag != null)
        {
           yield return StartCoroutine(FadeOut(chosenGroup, fadeOutDuration));
        }

        // 另一個（被淘汰的）→ 先 VFX，短暫停頓後才拋起淡出
        if (otherDrag != null)
        {
            otherDrag.ResetPosition();

            // 1) 先劈
            if (vfxManager != null)
            {
                Vector2 worldPos = GetWorldPosition(otherDrag.transform as RectTransform);
                vfxManager.SpawnSlashVFX(worldPos,90f);
            }


            // 3) 被劈後才飛起來淡出
            StartCoroutine(ThrowArcRoutine(
                otherDrag.transform as RectTransform,
                peakHeight: 300f,
                duration: 0.5f,
                horizontalDrift: 20f));

            yield return StartCoroutine(FadeOut(otherGroup, fadeOutDuration));

            onResolveComplete?.Invoke();
            onResolveComplete = null;
        }
    }


    /// <summary>
    /// 兩個選項都播放 VFX 並淡出（例如：強制事件、無法選擇的結局）
    /// 呼叫此方法後，兩個選項立即禁用拖曳互動。
    /// </summary>
    public void ResolveChoiceBothVFX()
    {
        if (!waitingForChoice) return; // 防止重複觸發
        waitingForChoice = false;

        // 立即停用兩個選項的拖曳互動
        optionADrag?.SetInteractable(false);
        optionBDrag?.SetInteractable(false);

        StartCoroutine(ResolveBothVFX());
    }

    private IEnumerator ResolveBothVFX()
    {
        optionADrag?.ResetPosition();
        optionBDrag?.ResetPosition();

        // 兩個選項同時拋起來（parallel coroutines）
        Coroutine arcA = null, arcB = null;

        if (optionADrag != null)
            arcA = StartCoroutine(ThrowArcRoutine(
                optionADrag.transform as RectTransform,
                peakHeight: 100f,
                duration: 0.5f,
                horizontalDrift: -30f));   // 稍微往左漂

        if (optionBDrag != null)
            arcB = StartCoroutine(ThrowArcRoutine(
                optionBDrag.transform as RectTransform,
                peakHeight: 100f,
                duration: 0.5f,
                horizontalDrift: 30f));    // 稍微往右漂

        // 等拋物線動畫結束（等較長的那個）
        if (arcA != null) yield return arcA;
        if (arcB != null) yield return arcB;

        // 拋到頂後生成 VFX
        if (vfxManager != null)
        {
            if (optionADrag != null)
                vfxManager.SpawnSlashVFX(GetWorldPosition(optionADrag.transform as RectTransform));
            if (optionBDrag != null)
                vfxManager.SpawnSlashVFX(GetWorldPosition(optionBDrag.transform as RectTransform));
        }

        yield return new WaitForSeconds(0.15f);

        // 同時淡出
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            if (optionAGroup) optionAGroup.alpha = alpha;
            if (optionBGroup) optionBGroup.alpha = alpha;
            yield return null;
        }
        SetOptionVisible(optionAGroup, false);
        SetOptionVisible(optionBGroup, false);
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

    /// <summary>同時將兩個 option CanvasGroup 從 0 淡入到 1，null 代表跳過</summary>
    private IEnumerator FadeInOptions(CanvasGroup groupA, CanvasGroup groupB, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            if (groupA != null) groupA.alpha = alpha;
            if (groupB != null) groupB.alpha = alpha;
            yield return null;
        }
        if (groupA != null)
        {
            groupA.alpha = 1f;
            groupA.blocksRaycasts = true;
            groupA.interactable = true;
        }
        if (groupB != null)
        {
            groupB.alpha = 1f;
            groupB.blocksRaycasts = true;
            groupB.interactable = true;
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

    /// <summary>
    /// 讓 UI RectTransform 做出向上拋再落下的效果（不需要 Rigidbody2D）
    /// </summary>
    /// <param name="rt">要播放動畫的 RectTransform</param>
    /// <param name="peakHeight">最高點距離原點的高度（正值 = 向上，單位：px）</param>
    /// <param name="duration">整段拋物線時長（秒）</param>
    /// <param name="horizontalDrift">水平漂移量（正值 = 向右）</param>
    private IEnumerator ThrowArcRoutine(
        RectTransform rt,
        float peakHeight = 120f,
        float duration = 0.6f,
        float horizontalDrift = 0f)
    {
        Vector2 startPos = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 垂直：拋物線公式 y = 4h * t * (1-t)，t=0.5 時達到 peakHeight
            float yOffset = 4f * peakHeight * t * (1f - t);

            // 水平：等速漂移
            float xOffset = horizontalDrift * t;

            rt.anchoredPosition = startPos + new Vector2(xOffset, yOffset);
            yield return null;
        }

        // 確保落回原始位置
        rt.anchoredPosition = startPos;
    }

    #endregion
}