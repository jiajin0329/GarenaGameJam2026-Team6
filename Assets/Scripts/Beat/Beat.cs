using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Beat
    {
        [SerializeField]
        private BeatModel _beatModel;

        private BeatView _beatView;
        private BeatService _beatService;

        private Action _onBeatEvent;
        public void AddBeatListener(Action _action) => _onBeatEvent += _action;
        public void RemoveBeatListener(Action _action) => _onBeatEvent -= _action;

        private Action _onOneTimeBeatEvent;
        public void AddOneTimeBeatListener(Action _action) => _onOneTimeBeatEvent += _action;
        public void RemoveOneTimeBeatListener(Action _action) => _onOneTimeBeatEvent -= _action;

        public Beat(LevelConfig _levelConfig)
        {
            _beatModel = new(_levelConfig.bpm, _levelConfig.oneTimeBeatAmount);
            _beatView = new();
            _beatService = new BeatService(_beatModel);
        }

        public void Tick(float _deltaTime)
        {
            _beatModel.Tick(_deltaTime);

            if (_beatService.CanBeat())
            {
                _beatView.Beat();
                _onBeatEvent?.Invoke();
                Debug.Log(nameof(_beatService.CanBeat));
            }

            if (_beatService.CanOneTimeBeat())
            {
                _beatView.OneTimeBeat();
                _onOneTimeBeatEvent?.Invoke();
                Debug.Log(nameof(_beatService.CanOneTimeBeat));
            }
        }
    }
}

