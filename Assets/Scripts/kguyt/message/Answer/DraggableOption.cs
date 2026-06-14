using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using Logy.UnityCommonV01;
/// <summary>
/// 掛在 optionA / optionB GameObject 上。
/// 需要同一 GameObject 上有 CanvasGroup（Inspector 自動抓取）。
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DraggableOption : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ── 回呼（由 DialogueChoiceController 注入） ────────────────────
    public Action onDropped;
    // ── 狀態 ────────────────────────────────────────────────────────
    private bool interactable = false;
    private Canvas rootCanvas;
    private Vector2 startAnchoredPos;   // 拖曳開始時的位置（距離判斷用）
    private Vector2 originAnchoredPos;  // Editor 設定的原始位置（歸位用）
    private Quaternion originalLocalRotation; // Editor 設定的原始旋轉（歸位用）
    private RectTransform rt;
    public CharacterViewSwitcher characterViewSwitcher;
    public Action oneTimeAnswerEvnet;

    [Header("拖曳握持效果")]
    [SerializeField] private float dragTiltAngle = 30f;   // 握住時傾斜角度
    [SerializeField] private float dragTiltSpeed = 0.02f;   // 傾斜過渡速度（越大越快）


    [Header("放下衝擊效果")]
    [SerializeField] private float dropImpactDistance = 25f;   // 放下時下沉距離（px）
    [SerializeField] private float dropImpactDuration = 0.08f; // 下沉時長
    [SerializeField] private Vector2 dropSquashScale = new Vector2(1.15f, 0.85f); // 下沉時的壓扁比例
    private Coroutine tiltRoutine;

    // ═══════════════════════════════════════════════════════════════
    #region Unity Lifecycle
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        originAnchoredPos = rt.anchoredPosition;       // ← 記錄 Editor 設定的原始位置
        originalLocalRotation = rt.localRotation;      // ← 記錄 Editor 設定的原始旋轉
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

        // 開始拖曳時，疊加一個隨機方向的傾斜角度，模擬被抓起握歪的感覺
        SFXPlayer.instance.PlayOneShot(AudioName.catchEf);
        float baseZ = originalLocalRotation.eulerAngles.z;
        if (baseZ > 180f) baseZ -= 360f;

        // 隨機決定本次傾斜是正還是負
        float randomSign = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        float targetZ = baseZ + randomSign * dragTiltAngle;

        StartTilt(Quaternion.Euler(0f, 0f, targetZ));
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
        interactable = false;
        Debug.Log(gameObject.name);

        StartCoroutine(DropImpactRoutine());

        oneTimeAnswerEvnet?.Invoke();
        oneTimeAnswerEvnet = null;
    }

    /// <summary>放下時的「咚」一下衝擊感：快速下沉並壓扁，不回彈</summary>
    private IEnumerator DropImpactRoutine()
    {
        if (tiltRoutine != null)
        {
            StopCoroutine(tiltRoutine);
            tiltRoutine = null;
        }

        Vector2 currentPos = rt.anchoredPosition;
        Vector3 normalScale = Vector3.one;
        Vector3 squashScale = new Vector3(dropSquashScale.x, dropSquashScale.y, 1f);

        // 快速下沉 + 壓扁
        float elapsed = 0f;
        while (elapsed < dropImpactDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropImpactDuration;
            rt.anchoredPosition = currentPos + new Vector2(0f, -dropImpactDistance * t);
            rt.localScale = Vector3.Lerp(normalScale, squashScale, t);
            yield return null;
        }

        rt.anchoredPosition = currentPos + new Vector2(0f, -dropImpactDistance);
        rt.localScale = squashScale;

        // 衝擊動畫播完後，才通知 DialogueChoiceController 開始淡出/掉落流程
        onDropped?.Invoke();
    }
    #endregion
    // ═══════════════════════════════════════════════════════════════
    #region 工具

    /// <summary>啟動／切換旋轉過渡到指定目標角度</summary>
    private void StartTilt(Quaternion targetRot)
    {
        if (tiltRoutine != null) StopCoroutine(tiltRoutine);
        tiltRoutine = StartCoroutine(TiltRoutine(targetRot));
    }

    private IEnumerator TiltRoutine(Quaternion targetRot)
    {
        while (Quaternion.Angle(rt.localRotation, targetRot) > 0.1f)
        {
            rt.localRotation = Quaternion.Lerp(rt.localRotation, targetRot, Time.deltaTime * dragTiltSpeed);
            yield return null;
        }
        rt.localRotation = targetRot;
        tiltRoutine = null;
    }

    /// <summary>重設位置與旋轉回 Editor 設定的原始狀態</summary>
    public void ResetPosition()
    {
        if (tiltRoutine != null)
        {
            StopCoroutine(tiltRoutine);
            tiltRoutine = null;
        }
        rt.anchoredPosition = originAnchoredPos;
        rt.localRotation = originalLocalRotation;
        rt.localScale = Vector3.one;   // ← 補上比例歸位
    }
    #endregion
}