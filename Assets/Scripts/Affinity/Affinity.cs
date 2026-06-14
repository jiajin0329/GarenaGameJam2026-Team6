using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Affinity
    {
        [field: SerializeField]
        public AffinityModel model { get; private set; }

        [field: SerializeField]
        private AffinityBar _bar;

        public Affinity(float _current, float _max, AffinityBar _bar)
        {
            model = new AffinityModel(_current, _max);
            this._bar = _bar;
        }

        public void Set(float _set)
        {
            model.Set(_set);
            _bar.SetAffinity(model.normalizedCurrent);
        }

        public void Change(float _change) => Set(model.current + _change);

        public void ShowUI() => _bar.gameObject.SetActive(true);

        public void HideUI() => _bar.gameObject.SetActive(false);
    }
}

