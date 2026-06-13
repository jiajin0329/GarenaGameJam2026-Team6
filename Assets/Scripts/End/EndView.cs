using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class EndView
    {
        [SerializeField]
        private EndSplider[] _endSliderArrary;

        private Affinity[] _affinityArrary;

        public void Initialize(Affinity[] _affinityArrary)
        {
            this._affinityArrary = _affinityArrary;

            foreach (var _endSlider in _endSliderArrary)
            {
                _endSlider.Initialize();
            }
        }

        public async UniTaskVoid UpdateAffinity()
        {
            for (int i = 0; i < _affinityArrary.Length; i++)
            {
                await _endSliderArrary[i].SetAffinity((int)_affinityArrary[i].model.current);
            }
        }
    }
}
