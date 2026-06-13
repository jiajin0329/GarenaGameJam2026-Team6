using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 掛在 optionA / optionB GameObject 上。
/// 需要同一 GameObject 上有 CanvasGroup（Inspector 自動抓取）。
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DraggableOption : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ── 回呼（由 DialogueChoiceController 注入） ────────────────────
    private Action onDropped;

    // ── 狀態 ────────────────────────────────────────────────────────
    private bool interactable = false;
    private Canvas rootCanvas;
    private Vector2 startAnchoredPos;   // 拖曳開始時的位置（距離判斷用）
    private Vector2 originAnchoredPos;  // Editor 設定的原始位置（歸位用）
    private RectTransform rt;

    public CharacterViewSwitcher characterViewSwitcher;


    public Action oneTimeAnswerEvnet;

    // ═══════════════════════════════════════════════════════════════
    #region Unity Lifecycle

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        originAnchoredPos = rt.anchoredPosition;   // ← 記錄 Editor 設定的原始位置
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 公開 API

    public void SetInteractable(bool value)
    {
        interactable = value;
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = value;
    }

    /// <summary>注入放手後的回呼（每次 PlayDialogue 都會重新注入）</summary>
    public void SetOnDropped(Action callback) => onDropped = callback;

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region Drag Handlers

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        startAnchoredPos = rt.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!interactable) return;
        if (rootCanvas == null) return;

        // 跟著滑鼠 / 觸控移動
        rt.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!interactable) return;

        // 拖曳距離超過閾值才算有效選擇，否則彈回原位
        float dragDistance = Vector2.Distance(rt.anchoredPosition, startAnchoredPos);
        if (dragDistance >= 40f)
        {
            // 有效放手 → 觸發回呼
            interactable = false;
            Debug.Log(gameObject.name);
            onDropped?.Invoke();

            oneTimeAnswerEvnet?.Invoke();
        }
        else
        {
            // 距離不足 → 彈回原位
            rt.anchoredPosition = startAnchoredPos;
        }
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 工具

    /// <summary>重設位置回 Editor 設定的原始位置</summary>
    public void ResetPosition() => rt.anchoredPosition = originAnchoredPos;

    #endregion
}