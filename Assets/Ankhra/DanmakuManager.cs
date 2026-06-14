using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DanmakuManager : MonoBehaviour
{
    [Header("用於控制彈幕是否開啟")]
    public bool isSpawningDanmakuFlag = true;

    public GameObject[] Danmakus;

    public Transform spawnArea_DL;
    public Transform spawnArea_TR;

    public float danmakuRandomCD_Min = 0.4f;
    public float danmakuRandomCD_Max = 1f;

    public float danmakuCountingDownCD = 0;

    public void Update()
    {
        if (!isSpawningDanmakuFlag)
        {
            return;
        }
        danmakuCountingDownCD -= Time.deltaTime;

        if (danmakuCountingDownCD < 0)
        {
            LaunchDanmaku();
            danmakuCountingDownCD = 0;
            danmakuCountingDownCD += Random.Range(danmakuRandomCD_Min, danmakuRandomCD_Max);
        }
    }

    public void LaunchDanmaku()
    {
        GameObject damakuToLaunch = Danmakus[Random.Range(0, Danmakus.Length)];

        //Spawn pos
        Vector2 position = new Vector2(
            Random.Range(spawnArea_DL.position.x, spawnArea_TR.position.x),
            Random.Range(spawnArea_DL.position.y, spawnArea_TR.position.y)
            );

        Instantiate(damakuToLaunch, position, Quaternion.identity);
    }
}
