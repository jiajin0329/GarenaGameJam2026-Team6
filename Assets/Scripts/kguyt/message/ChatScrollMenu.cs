using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.InputSystem;

/// <summary>
/// 掛在 ScrollView 根物件上。
/// 提供 OnMessageReceived(string) 可在 Inspector 或程式端呼叫，
/// 動態生成訊息 Prefab 並將 ScrollRect 滾到最底。
/// </summary>
public class ChatScrollMenu : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ScrollRect 元件")]
    [SerializeField] private ScrollRect scrollRect;

    [Tooltip("Prefab：單則訊息（需含 Text 或 TMP_Text 元件）")]
    [SerializeField] private GameObject messagePrefab;

    [Tooltip("Content RectTransform（Vertical Layout Group 掛在這裡）")]
    [SerializeField] private RectTransform content;

    [Header("Event — 外部呼叫這個來新增訊息")]
    /// <summary>
    /// 將此 UnityEvent 暴露在 Inspector，
    /// 也可在程式中直接呼叫 chatScrollMenu.OnMessageReceived.Invoke("文字")
    /// </summary>
    public StringUnityEvent OnMessageReceived;

    // -------------------------------------------------------

    private void Awake()
    {
        // 確保 Event 已初始化（Inspector 沒設定時也能用）
        if (OnMessageReceived == null)
            OnMessageReceived = new StringUnityEvent();

        // 把實際處理邏輯註冊進去
        OnMessageReceived.AddListener(SpawnMessage);
    }

    
    /// <summary>
    /// 生成一則訊息並滾到底部。
    /// 可直接呼叫，也可透過 OnMessageReceived 觸發。
    /// </summary>
    public void SpawnMessage(string message)
    {
        if (messagePrefab == null || content == null)
        {
            Debug.LogWarning("[ChatScrollMenu] messagePrefab 或 content 未設定！");
            return;
        }

        // 1. 生成 Prefab
        GameObject newMsg = Instantiate(messagePrefab, content);

        // 2. 設定文字（支援 uGUI Text 與 TextMeshPro）
        SetMessageText(newMsg, message);

        // 3. 等一幀讓 Layout Group 重算後再滾動
        StartCoroutine(ScrollToBottomNextFrame());
    }

    // -------------------------------------------------------
    // 私有輔助

    private void SetMessageText(GameObject msgObj, string text)
    {
        // 優先嘗試 TextMeshPro
        var tmp = msgObj.GetComponentInChildren<TMPro.TMP_Text>();
        if (tmp != null) { tmp.text = text; return; }
        // Fallback：uGUI Text

        Debug.LogWarning("[ChatScrollMenu] Message Prefab 找不到 Text 或 TMP_Text 元件！");
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        // 等兩幀確保 ContentSizeFitter / LayoutGroup 都已更新
        yield return null;
        yield return null;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // 0 = 底部，1 = 頂部
    }
}

// -------------------------------------------------------
// 讓 UnityEvent<string> 可在 Inspector 序列化
[System.Serializable]
public class StringUnityEvent : UnityEvent<string> { }
