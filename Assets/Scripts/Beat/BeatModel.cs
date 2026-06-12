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

        public BeatModel(int _bpm, int _oneTimeBeatAmount)
        {
            bpm = _bpm;
            oneTimeBeatAmount = _oneTimeBeatAmount;
            beatInterval = 60f / _bpm;
        }

        public void Tick(float _deltaTime)
        {
            timer += _deltaTime;
        }

        public void ClearTimer()
        {
            timer -= beatInterval;
        }
    }
}

