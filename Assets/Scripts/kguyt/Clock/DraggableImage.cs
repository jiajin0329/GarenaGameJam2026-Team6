using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 拖曳 UI Image，放手後觸發 OnDragReleased 事件，並讓 Image 返回原始位置。
/// 掛載於 Canvas (World Space) 底下的 Image GameObject 上。
/// </summary>
public class DraggableImage : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("返回動畫")]
    [Tooltip("返回原始位置所需時間（秒）")]
    [SerializeField] private float returnDuration = 0.4f;

    [Tooltip("返回動畫曲線（預設為 EaseOut）")]
    [SerializeField] private AnimationCurve returnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

   
    // ── 內部狀態 ──────────────────────────────────────────
    private RectTransform _rectTransform;
    private Canvas        _canvas;
    private Vector2       _originalLocalPosition;
    private Coroutine     _returnCoroutine;
    private bool          _isDragging = false;

    // ── 初始化 ────────────────────────────────────────────
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // 往上找到最近的 Canvas
        _canvas = GetComponentInParent<Canvas>();

        if (_canvas == null)
            Debug.LogError("[DraggableImage] 找不到父層 Canvas，請確認掛載位置。");

        if (_canvas != null && _canvas.renderMode != RenderMode.WorldSpace)
            Debug.LogWarning("[DraggableImage] Canvas 不是 World Space 模式，座標換算可能有偏差。");
    }

    private void Start()
    {
        // 記錄初始 Local Position（相對於父物件）
        _originalLocalPosition = _rectTransform.localPosition;
    }

    // ── 拖曳開始 ──────────────────────────────────────────
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果正在返回，先打斷
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        _isDragging = true;
    }

    // ── 拖曳中：讓 Image 跟隨滑鼠 ────────────────────────
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // World Space Canvas 需要用 Camera 做螢幕→世界座標換算
        Camera eventCamera = eventData.pressEventCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,   // 轉換到父物件的 local 空間
                eventData.position,
                eventCamera,
                out Vector2 localPoint))
        {
            _rectTransform.localPosition = localPoint;
        }
    }

    // ── 拖曳結束：觸發事件 + 開始返回 ────────────────────
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;


        // 啟動返回動畫
        _returnCoroutine = StartCoroutine(ReturnToOrigin());

        StartCoroutine(TimeStop());
    }

    // ── 返回動畫 Coroutine ────────────────────────────────
    private IEnumerator ReturnToOrigin()
    {
        Vector2 startPosition = _rectTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float curveT = returnCurve.Evaluate(t);

            _rectTransform.localPosition = Vector2.Lerp(startPosition, _originalLocalPosition, curveT);
            yield return null;
        }

        // 確保精準落在原始位置
        _rectTransform.localPosition = _originalLocalPosition;
        _returnCoroutine = null;
    }

    // ── 公開方法：外部強制重設位置（選用）────────────────
    /// <summary>立即將 Image 移回原始位置（不播放動畫）</summary>
    public void ResetPositionImmediate()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
        _rectTransform.localPosition = _originalLocalPosition;
    }

    /// <summary>更新「原始位置」為當前位置（如需在執行期重新定位）</summary>
    public void SetCurrentPositionAsOrigin()
    {
        _originalLocalPosition = _rectTransform.localPosition;
    }

    IEnumerator TimeStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(4f);
        Time.timeScale = 1f;
    }
}
