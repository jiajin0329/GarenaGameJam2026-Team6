using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class LevelManager : Logy.UnityCommon.ProgressV01.Progress
    {
        [field: SerializeField]
        public LevelConfig _config { get; private set; }

        private Beat _beat;

        public void AddOnBeatEvent(Action _action) => _beat.AddOnBeatEvent(_action);
        public void RemoveOnBeatEvent(Action _action) => _beat.RemoveOnBeatEvent(_action);
        public void AddOnOneTimeBeatEvent(Action _action) => _beat.AddOnOneTimeBeatEvent(_action);
        public void RemoveOnOneTimeBeatEvent(Action _action) => _beat.RemoveOnOneTimeBeatEvent(_action);

        public override void Initialize()
        {
            _beat = new(_config);
        }

        private void Update()
        {
            _beat.Tick(Time.deltaTime);
        }
    }
}
