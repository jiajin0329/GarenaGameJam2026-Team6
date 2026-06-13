using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Beat
    {
        [SerializeField]
        private BeatModel _model;

        [SerializeField]
        private BeatView _view;

        private BeatService _service;

        private Action _onBeatEvent;
        public void AddBeatListener(Action _action) => _onBeatEvent += _action;
        public void RemoveBeatListener(Action _action) => _onBeatEvent -= _action;

        private Action _onOneTimeBeatEvent;
        public void AddOneTimeBeatListener(Action _action) => _onOneTimeBeatEvent += _action;
        public void RemoveOneTimeBeatListener(Action _action) => _onOneTimeBeatEvent -= _action;

        public void Initialize(LevelConfig _levelConfig)
        {
            _model = new(_levelConfig);
            _service = new BeatService(_model);
            _view.Initialize(_levelConfig);
        }

        public void Tick(float _deltaTime)
        {
            _model.Tick(_deltaTime);

            if (!_service.CanBeat())
                return;

            _model.Beat();
            _view.Beat(_model);
            _view.TickRemainingBeatSlider(_model);

            if (_model.beatCount != _model.oneTimeBeatAmount)
                _onBeatEvent?.Invoke();

            if (_service.CanOneTimeBeat())
            {
                _view.OneTimeBeat(_model);
                _onOneTimeBeatEvent?.Invoke();
                _model.ResetBeatCount();
                Debug.Log(nameof(_service.CanOneTimeBeat));
            }
        }

        public void ResetBeat()
        {
            _model.ResetBeatCount();
            _view.RsetBeatCount();
            Debug.Log(nameof(ResetBeat));
        }

        public void Reset()
        {
            _model.Reset();
            _view.Reset();
        }
    }
}

