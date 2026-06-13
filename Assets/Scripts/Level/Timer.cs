using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Timer
    {
        private float _timer = 0f;
        private float _finishTime = 0f;

        [SerializeField]
        private float _normalizedCurrentTime = 0f;

        public Timer(LevelConfig _config)
        {
            this._finishTime = 60f / _config.bpm * _config.questionIntervalBeatAmount * _config.questionCount;
        }

        public void Tick(float _deltaTime)
        {
            _timer += _deltaTime;
            UpdateUI();
        }


        private void UpdateUI()
        {
            _normalizedCurrentTime = _timer / _finishTime;
        }
    }
}
