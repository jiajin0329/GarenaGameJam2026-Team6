using UnityEngine;
using System.Collections;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public SO_Story LoadingStory;

    [Header("Test Container")]
    public bool isLoadOnStart;
    public SO_Story StoryLoadOnStart;


    [Header("ui")]
    public string[] conLoading;
    public string conSpeaking;
    public float interval = 0.1f;

    public TextMeshProUGUI ConversationTMP;

    [Header("Flags")]
    public bool NextConversationFlags;

    public void Start()
    {
        if (isLoadOnStart)
        {
            StartStory(StoryLoadOnStart);
        }
    }

    public void Update()
    {
        ConversationTMP.text = conSpeaking;
    }

    public void StartStory(SO_Story story)
    {
        conLoading = story.context;
        StartCoroutine(StoryCoroutine());
    }

    public IEnumerator StoryCoroutine()
    {
        for (int i = 0; i < conLoading.Length; i++)
        {
            NextConversationFlags = false;
            conSpeaking = "";
            for (int j = 0; j < conLoading[i].Length; j++)
            {
                //showing text
                conSpeaking += conLoading[i][j];
                yield return new WaitForSeconds(interval);
            }
            //fin loading
            yield return new WaitUntil(()=> NextConversationFlags);
        }
    }

    public void ReadNextLine()
    {
        NextConversationFlags = true;
    }
}
