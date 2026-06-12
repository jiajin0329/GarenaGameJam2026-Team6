using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// 管理 AllScreen 的滑動切換，以及向各 DialogueChoiceController 發送對話指令。
/// 測試快捷鍵：
///   1 / 2 / 3 → 切換到對應角色視窗
///   T         → 觸發測試對話（角色 B，index=1）
/// </summary>
public class CharacterViewSwitcher : MonoBehaviour
{
    // ── Inspector 綁定 ──────────────────────────────────────────────
    [Header("UI 父層（AllScreen）")]
    [SerializeField] private RectTransform uiParent;

    [Header("滑動設定")]
    [SerializeField] private float transitionDuration = 0.4f;

    [Header("角色 Controllers")]
    [SerializeField] private DialogueChoiceController controllerA;
    [SerializeField] private DialogueChoiceController controllerB;
    [SerializeField] private DialogueChoiceController controllerC;

    // ── 常數 ────────────────────────────────────────────────────────
    private readonly float[] targetPositions = { 637f, 0f, -637f };

    // ── 私有狀態 ────────────────────────────────────────────────────
    private DialogueChoiceController currentController;
    private bool isSwitching;

    // ── Input Actions（新版 Input System） ──────────────────────────
    private InputAction key1, key2, key3, keyTest;

    // ═══════════════════════════════════════════════════════════════
    #region Unity Lifecycle

    private void Awake()
    {
        key1 = new InputAction("SwitchA", binding: "<Keyboard>/1");
        key2 = new InputAction("SwitchB", binding: "<Keyboard>/2");
        key3 = new InputAction("SwitchC", binding: "<Keyboard>/3");
        keyTest = new InputAction("TestDialogue", binding: "<Keyboard>/t");
    }

    private void OnEnable()
    {
        key1.performed += _ => TrySwitchTo(0);
        key2.performed += _ => TrySwitchTo(1);
        key3.performed += _ => TrySwitchTo(2);
        keyTest.performed += _ => TriggerTestDialogue();

        key1.Enable();
        key2.Enable();
        key3.Enable();
        keyTest.Enable();
    }

    private void OnDisable()
    {
        key1.Disable();
        key2.Disable();
        key3.Disable();
        keyTest.Disable();
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 公開 API

    /// <summary>
    /// 主要對外接口：切換到指定角色並播放對話。
    /// </summary>
    /// <param name="text">對話內容</param>
    /// <param name="characterIndex">角色索引（0=A, 1=B, 2=C）</param>
    /// <param name="optionAText">選項 A 文字，null 表示不顯示</param>
    /// <param name="optionBText">選項 B 文字，null 表示不顯示</param>
    public void PlayDialogue(string text, int characterIndex,
                             string optionAText = null, string optionBText = null)
    {
        if (isSwitching)
        {
            Debug.Log("[Switcher] 正在切換中，請稍後...");
            return;
        }
        StartCoroutine(SwitchAndPlay(text, characterIndex, optionAText, optionBText));
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 切換邏輯

    private void TrySwitchTo(int index)
    {
        if (isSwitching)
        {
            Debug.Log("[Switcher] 正在切換中，請稍後...");
            return;
        }
        StartCoroutine(SwitchCoroutine(index));
    }

    private IEnumerator SwitchAndPlay(string text, int index,
                                      string optionAText, string optionBText)
    {
        // 先滑動到目標角色
        yield return StartCoroutine(SwitchCoroutine(index));

        // 滑動完成後，要求目標 Controller 播放對話
        currentController?.PlayDialogue(text, optionAText, optionBText);
    }

    private IEnumerator SwitchCoroutine(int index)
    {
        isSwitching = true;

        // 停用前一個 Controller
        if (currentController != null)
            currentController.Deactivate();

        // 滑動 AllScreen
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

        // 切換 currentController
        currentController = index switch
        {
            0 => controllerA,
            1 => controllerB,
            2 => controllerC,
            _ => null
        };

        currentController?.Activate();
        isSwitching = false;

        Debug.Log($"[Switcher] 已切換到角色 {(char)('A' + index)}");
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    #region 測試

    /// <summary>
    /// 按 T 觸發：切換到角色 B（index=1），顯示測試對話和兩個選項。
    /// 模擬你提到的「我們今晚一起去哪, 1」情境。
    /// </summary>
    private void TriggerTestDialogue()
    {
        Debug.Log("[Test] 觸發測試對話");
        PlayDialogue(
            text: "我們今晚一起去哪？",
            characterIndex: Random.Range(0, 3),
            optionAText: "去看電影吧",
            optionBText: "不了，我有事"
        );
    }

    #endregion
}