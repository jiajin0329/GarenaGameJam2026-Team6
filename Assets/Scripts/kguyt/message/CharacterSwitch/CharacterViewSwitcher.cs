using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterViewSwitcher : MonoBehaviour
{
    [Header("UI 母物件")]
    [SerializeField] private RectTransform uiParent;

    [Header("切換動畫")]
    [SerializeField] private float transitionDuration = 0.4f;

    [Header("角色 Controllers")]
    [SerializeField] private DialogueChoiceController controllerA;
    [SerializeField] private DialogueChoiceController controllerB;
    [SerializeField] private DialogueChoiceController controllerC;

    private readonly float[] targetPositions = { 637f, 0f, -637f };
    private DialogueChoiceController currentController = null;
    private bool isSwitching = false;

    private InputAction key1, key2, key3;

    // ────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        key1 = new InputAction("SwitchA", binding: "<Keyboard>/1");
        key2 = new InputAction("SwitchB", binding: "<Keyboard>/2");
        key3 = new InputAction("SwitchC", binding: "<Keyboard>/3");
    }

    private void OnEnable()
    {
        key1.performed += _ => TrySwitchTo(0);
        key2.performed += _ => TrySwitchTo(1);
        key3.performed += _ => TrySwitchTo(2);

        key1.Enable();
        key2.Enable();
        key3.Enable();
    }

    private void OnDisable()
    {
        key1.Disable();
        key2.Disable();
        key3.Disable();
    }

    #endregion

    // ────────────────────────────────────────
    #region 切換邏輯

    private void TrySwitchTo(int index)
    {
        if (isSwitching)
        {
            Debug.Log("[Switcher] 切換中，請稍候...");
            return;
        }
        StartCoroutine(SwitchCoroutine(index));
    }

    private IEnumerator SwitchCoroutine(int index)
    {
        isSwitching = true;

        // 停用當前 Controller
        if (currentController != null)
            currentController.Deactivate();

        // 移動 UI
        Vector2 startPos = uiParent.anchoredPosition;
        Vector2 endPos = new Vector2(targetPositions[index], startPos.y);
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            uiParent.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        uiParent.anchoredPosition = endPos;

        // 啟用目標 Controller
        currentController = index switch
        {
            0 => controllerA,
            1 => controllerB,
            2 => controllerC,
            _ => null
        };

        if (currentController != null)
            currentController.Activate();

        isSwitching = false;
        Debug.Log($"[Switcher] 已切換到角色 {(char)('A' + index)}");
    }

    #endregion
}