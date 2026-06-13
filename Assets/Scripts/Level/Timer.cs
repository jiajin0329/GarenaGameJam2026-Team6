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

        [SerializeField]
        private RectTransform _pointerRectTransform;

        private float _startLocalAngleZ;
        private Vector3 _localAngle;

        public void Initialize(LevelConfig _config)
        {
            _startLocalAngleZ = _pointerRectTransform.localEulerAngles.z;
            _finishTime = 60f / _config.bpm * _config.questionIntervalBeatAmount * _config.questionCount;
        }

        public void Tick(float _deltaTime)
        {
            _timer += _deltaTime;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _normalizedCurrentTime = _timer / _finishTime;
            _localAngle.z = _startLocalAngleZ - _normalizedCurrentTime * 180f;
            _pointerRectTransform.localEulerAngles = _localAngle;
        }

        public void GetRemainingTime(float _remainingTime)
        {
            _timer += _remainingTime;
            Debug.Log(nameof(GetRemainingTime));
        }
    }
}
