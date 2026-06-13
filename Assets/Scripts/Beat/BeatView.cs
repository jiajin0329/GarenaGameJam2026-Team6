using System;
using DG.Tweening;
using Logy.UnityCommonV01;
using UnityEngine;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class BeatView
    {
        [field: SerializeField]
        public int remainingBeatCount { get; private set; }

        [field: SerializeField]
        public float normalized { get; private set; }

        [field: SerializeField]
        public Slider[] _remainingBeatSlider { get; private set; }

        private LevelConfig _levelConfig;
        private int _beatCount;

        public void Initialize(LevelConfig _levelConfig)
        {
            this._levelConfig = _levelConfig;
        }

        public void Beat(BeatModel _model)
        {
            _beatCount++;

            if (_beatCount == _model.oneTimeBeatAmount)
                return;

            SFXPlayer.instance.PlayOneShot(AudioName.beat);
        }

        public void TickRemainingBeatSlider(BeatModel _model)
        {
            remainingBeatCount = _model.questionIntervalBeatAmount - _beatCount;
            normalized = (float)remainingBeatCount / _model.questionIntervalBeatAmount;
            SetRemainingBeatSlider(normalized);
        }

        private void SetRemainingBeatSlider(float _normalized)
        {
            for (int i = 0; i < _remainingBeatSlider.Length; i++)
            {
                // 先殺死該 Slider 身上正在執行的 Dotween 動畫，避免多個動畫互相拉扯
                _remainingBeatSlider[i].DOKill();

                // 執行新動畫，並設定為 OutQuad 曲線 (開頭快，結尾慢，看起來較自然)
                _remainingBeatSlider[i].DOValue(_normalized, _levelConfig.remainingBeatSliderSmoothDuration).SetEase(Ease.OutQuad);
            }
        }

        public void OneTimeBeat(BeatModel _model)
        {
            if (_beatCount == _model.questionIntervalBeatAmount)
                _beatCount = 0;

            SFXPlayer.instance.PlayOneShot(AudioName.oneTimeBeat);
        }
    }
}

