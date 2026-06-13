using System;
using UnityEngine;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Timer
    {
        private float _timer = 0f;
        private float _finishTime = 0f;

        [field: SerializeField]
        public float normalizedCurrentTime { get; private set; } = 0f;

        [SerializeField]
        private RectTransform _pointerRectTransform;

        [SerializeField]
        private Image[] _dayImageArrary;

        private End _end;

        public int dayIndex = 0;
        private float _startLocalAngleZ;
        private Vector3 _localAngle;
        private bool _isEnable = true;

        public void Initialize(LevelConfig _config, End _end)
        {
            this._end = _end;
            _startLocalAngleZ = _pointerRectTransform.localEulerAngles.z;
            _finishTime = 60f / _config.bpm * _config.questionIntervalBeatAmount * _config.questionCount;

            for (int i = 0; i < _dayImageArrary.Length; i++)
            {
                _dayImageArrary[i].fillAmount = 0f;
            }
        }

        public void Tick(float _deltaTime)
        {
            if (!_isEnable)
                return;

            _timer += _deltaTime;
            UpdateUI();

            _dayImageArrary[_end.model.finishCount].fillAmount = normalizedCurrentTime;
        }

        private void UpdateUI()
        {
            normalizedCurrentTime = _timer / _finishTime;
            _localAngle.z = _startLocalAngleZ - normalizedCurrentTime * 180f;
            _pointerRectTransform.localEulerAngles = _localAngle;
        }

        public void GetRemainingTime(float _remainingTime)
        {
            _timer += _remainingTime;
            Debug.Log(nameof(GetRemainingTime));
        }

        public void IsEnableFalse()
        {
            _isEnable = false;
        }

        public void Reset()
        {
            _isEnable = true;
            _timer = 0f;
        }
    }
}
