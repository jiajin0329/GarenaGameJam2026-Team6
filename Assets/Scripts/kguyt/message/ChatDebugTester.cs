using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 測試用：按下鍵盤 [1] 觸發一則訊息。
/// 掛在場景任意物件上，將 chatScrollMenu 拖進 Inspector 即可。
/// </summary>
public class ChatDebugTester : MonoBehaviour
{
    [SerializeField] private ChatScrollMenu chatScrollMenu;

    [Tooltip("測試用的訊息內容")]
    [SerializeField] private string testMessage = "這是一則測試訊息！";

    private int _counter = 0;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            _counter++;
            chatScrollMenu.OnMessageReceived.Invoke($"[{_counter}] {testMessage}");
        }
    }
}
