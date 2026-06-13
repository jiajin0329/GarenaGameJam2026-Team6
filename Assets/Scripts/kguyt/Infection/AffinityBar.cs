using GarenaGameJam2026Team6;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AffinityBar : MonoBehaviour
{
    public float current_AffinityValue;
    public float max_AffinityValue = 10f;

    public float target_AffinityValue;

    public Image AffinityImage;

    public float smoothValue = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        current_AffinityValue = Mathf.Lerp(current_AffinityValue, target_AffinityValue, smoothValue * Time.deltaTime);
        AffinityImage.fillAmount = current_AffinityValue / 1;
    
    }

    public void SetTarget_AffinityValue(float affninityValue)
    {
        target_AffinityValue = affninityValue;
    }
}
