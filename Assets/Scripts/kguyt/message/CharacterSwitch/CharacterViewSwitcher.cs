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

    [Header("頭像 Images（A / B / C）")]
    [SerializeField] private RectTransform avatarA;
    [SerializeField] private RectTransform avatarB;
    [SerializeField] private RectTransform avatarC;

    [Header("頭像縮放設定")]
    [SerializeField] private float avatarScaleSelected = 1.5f;
    [SerializeField] private float avatarScaleNormal = 1.0f;
    [SerializeField] private float avatarScaleDuration = 0.25f;

    // ── 常數 ────────────────────────────────────────────────────────
    private readonly float[] targetPositions = { 643f, 0f, -648f };

    // ── 私有狀態 ────────────────────────────────────────────────────
    private DialogueChoiceController currentController;
    private bool isSwitching;

    // ── Input Actions（新版 Input System） ──────────────────────────
    private InputAction keyTest;

    // ═══════════════════════════════════════════════════════════════
    #region Unity Lifecycle

    private void Awake()
    {
        keyTest = new InputAction("TestDialogue", binding: "<Keyboard>/t");
    }

    private void OnEnable()
    {
        keyTest.performed += _ => TriggerTestDialogue();


        keyTest.Enable();
    }

    private void OnDisable()
    {
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

        if (currentController != null)
            currentController.Deactivate();

        // 滑動動畫（原有邏輯）
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

        // ── 新增：同步縮放所有頭像 ──────────────────────────
        RectTransform[] avatars = { avatarA, avatarB, avatarC };
        for (int i = 0; i < avatars.Length; i++)
        {
            if (avatars[i] == null) continue;
            float targetScale = (i == index) ? avatarScaleSelected : avatarScaleNormal;
            StartCoroutine(ScaleAvatarCoroutine(avatars[i], targetScale));
        }
        // ────────────────────────────────────────────────────

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

    private IEnumerator ScaleAvatarCoroutine(RectTransform avatar, float targetScale)
    {
        Vector3 startScale = avatar.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < avatarScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / avatarScaleDuration);
            avatar.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        avatar.localScale = endScale;
    }
}