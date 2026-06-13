using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Affinity
    {
        [field: SerializeField]
        public float current { get; private set; }

        [NonSerialized]
        public float max;

        public float normalizedCurrent { get; private set; }

        [SerializeField]
        private AffinityBar _affinityBar;

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
            _affinityBar.SetTarget_AffinityValue(normalizedCurrent);
        }

        public void Change(float _change) => Set(current + _change);
    }
}

