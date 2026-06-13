using UnityEngine;

/// <summary>
/// 視覺回饋接口。
/// 供協作者在選項判定完成後呼叫 OnJudgementResult()。
/// </summary>
public class VisualFeedbackReceiver : MonoBehaviour
{
    /// <summary>
    /// 判定完成後由外部呼叫。
    /// </summary>
    /// <param name="success">判定是否成功</param>
    public void OnJudgementResult(bool success)
    {
        // TODO：實作視覺回饋
        Debug.Log($"[VisualFeedback] 收到判定結果：{(success ? "成功" : "失敗")}");
    }
}