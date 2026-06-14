using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class AffinityModel
    {
        [field: SerializeField]
        public float current { get; private set; }

        [NonSerialized]
        public float max;

        public float normalizedCurrent { get; private set; }

        public AffinityModel(float _current, float _max)
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

