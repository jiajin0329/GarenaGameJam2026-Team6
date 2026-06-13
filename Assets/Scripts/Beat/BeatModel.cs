using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class BeatModel
    {
        [field: SerializeField]
        public int bpm { get; private set; }

        [field: SerializeField]
        public float beatInterval { get; private set; }

        [field: SerializeField]
        public int oneTimeBeatAmount { get; private set; }

        [field: SerializeField]
        public float timer { get; private set; }

        [field: SerializeField]
        public int beatCount { get; private set; }

        [field: SerializeField]
        public int questionIntervalBeatAmount { get; private set; }

        public BeatModel(LevelConfig _levelConfig)
        {
            bpm = _levelConfig.bpm;
            oneTimeBeatAmount = _levelConfig.oneTimeBeatAmount;
            questionIntervalBeatAmount = _levelConfig.questionIntervalBeatAmount;
            beatInterval = 60f / bpm;
        }

        public void Tick(float _deltaTime)
        {
            timer += _deltaTime;
        }

        public void Beat()
        {
            beatCount++;
            timer -= beatInterval;
        }

        public void ResetBeatCount()
        {
            beatCount = 0;
        }
    }
}

