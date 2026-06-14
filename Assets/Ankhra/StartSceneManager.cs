using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StartSceneManager : MonoBehaviour
{
    public GameObject[] StoryGameObject = new GameObject[18];
    public ScrollRect myScrollRect;

    public int totalStoryElementIndex = 18;

    public int currentIndex = 0;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float duration = 0.25f;

    public bool isPhoneGoingFlag = false;


    public void Start()
    {
        InitFunc();
    }

    public void Update()
    {

        if (!isPhoneGoingFlag)
        {
            return;
        }

        Debug.Log("Waht wver");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextPageEvent();
        }
    }

    public void InitFunc()
    {
        totalStoryElementIndex = StoryGameObject.Length;
    }

    public void NextPageEvent()
    {
        currentIndex += 1;
        if (currentIndex < totalStoryElementIndex)
        {
            //Load next page
            StoryGameObject[currentIndex].SetActive(true);
            ScrollToBottomSmooth();
        }
    }



    private Coroutine scrollCoroutine;

    public void ScrollToBottomSmooth()
    {
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(ScrollToBottomCoroutine());
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        // 等 UI Layout 更新
        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        float start = scrollRect.verticalNormalizedPosition;
        float end = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // SmoothStep 讓移動比較柔和
            t = Mathf.SmoothStep(0f, 1f, t);

            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, end, t);

            yield return null;
        }

        scrollRect.verticalNormalizedPosition = end;
        scrollCoroutine = null;
    }
}

