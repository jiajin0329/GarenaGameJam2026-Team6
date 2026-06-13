using UnityEngine;
using UnityEngine.UI;

public class AffinityBar : MonoBehaviour
{
    public float current_AffinityValue;
    public float max_AffinityValue = 10f;

    public float target_AffinityValue;

    public Image AffinityImage;

    public float smoothValue = 3f;
    void Update()
    {
        if (current_AffinityValue == target_AffinityValue) return;

        current_AffinityValue = Mathf.Lerp(current_AffinityValue, target_AffinityValue, smoothValue * Time.deltaTime);
        AffinityImage.fillAmount = current_AffinityValue / 1f;
    }

    public void SetAffinity(float newValue)
    {
        target_AffinityValue = newValue;
    }
}
