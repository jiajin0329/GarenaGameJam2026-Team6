using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Affinity
    {
        [field: SerializeField]
        public float current { get; private set; }

        public float max { get; private set; }
        public float normalizedCurrent { get; private set; }

        private Action<float> setNormalizedCurrentEvent;
        public void AddSetNormalizedCurrentListener(Action<float> _listener) => setNormalizedCurrentEvent += _listener;
        public void RemoveSetNormalizedCurrentListener(Action<float> _listener) => setNormalizedCurrentEvent -= _listener;

        public Affinity(float _current, float _max)
        {
            current = _current;
            max = _max;
        }

        public void Set(float _set)
        {
            current = _set;
            normalizedCurrent = current / max;
        }

        public void Change(float _change) => Set(current + _change);
    }
}

