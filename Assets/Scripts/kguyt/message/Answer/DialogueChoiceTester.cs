using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 測試用腳本，與 DialogueChoiceController 掛在同一 GameObject
/// 上線前可直接移除或關閉此腳本
/// </summary>
public class DialogueChoiceTester : MonoBehaviour
{
    [Header("目標 Controller")]
    [SerializeField] private DialogueChoiceController controller;

    [Header("測試文字內容")]
    [SerializeField] private string testQuestion = "這是一段測試對話，逐字顯示中...";
    [SerializeField] private string testSelection1 = "選擇 A";
    [SerializeField] private string testSelection2 = "選擇 B";

    private InputAction testTypeAction;
    private InputAction testChooseAAction;
    private InputAction testChooseBAction;
    private InputAction testBothSlashAction;

    private void Awake()
    {
        testTypeAction = new InputAction("TestType", binding: "<Keyboard>/t");
        testChooseAAction = new InputAction("TestChooseA", binding: "<Keyboard>/z");
        testChooseBAction = new InputAction("TestChooseB", binding: "<Keyboard>/x");
        testBothSlashAction = new InputAction("TestBothSlash", binding: "<Keyboard>/c");
    }

    private void OnEnable()
    {
        testTypeAction.performed += _ =>
        {
            controller.SetSelection1Text(testSelection1);
            controller.SetSelection2Text(testSelection2);
            controller.SetQuestionText(testQuestion);
        };

        testChooseAAction.performed += _ => controller.SetQuestionText("強制觸發：選A");
        testChooseBAction.performed += _ => controller.SetQuestionText("強制觸發：選B");
        testBothSlashAction.performed += _ => controller.TriggerBothSlash();

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
}