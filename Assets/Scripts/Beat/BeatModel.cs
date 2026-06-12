using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class BeatModel
    {
        public int bpm { get; private set; }
        public float beatInterval { get; private set; }
        public int oneTimeBeatCount { get; private set; }
        public float timer { get; private set; }

        public BeatModel(int _bpm, int _oneTimeBeatCount)
        {
            bpm = _bpm;
            oneTimeBeatCount = _oneTimeBeatCount;
            beatInterval = 60f / _bpm;
        }

        public void Tick(float _deltaTime)
        {
            timer += _deltaTime;
        }

        public void ClearTimer()
        {
            timer = 0f;
        }
    }
}

