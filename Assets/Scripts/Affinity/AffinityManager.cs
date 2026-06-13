using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class AffinityManager
    {
        public Affinity[] affinityArrary { get; private set; }

        [field: SerializeField]
        private Transform _instantiateParent;

        [field: SerializeField]
        private AffinityBar _prefab;

        public void Initialize(LevelConfig _config)
        {
            affinityArrary = new Affinity[3]
            {
                new Affinity(0, _config.affinityMax, GameObject.Instantiate(_prefab, _instantiateParent)),
                new Affinity(0, _config.affinityMax, GameObject.Instantiate(_prefab, _instantiateParent)),
                new Affinity(0, _config.affinityMax, GameObject.Instantiate(_prefab, _instantiateParent))
            };
        }

        public void Set(int _index, float _set)
        {
            affinityArrary[_index].Set(_set);
        }

        public void Change(int _index, float _change) => Set(_index, affinityArrary[_index].model.current + _change);

        public void ShowUI(int _index)
        {
            for (int i = 0; i < affinityArrary.Length; i++)
            {
                if (i == _index)
                    affinityArrary[i].ShowUI();
                else
                    affinityArrary[i].HideUI();
            }
        }

        public void HideUI(int _index) => affinityArrary[_index].HideUI();

        public void HideAllUI()
        {
            for (int i = 0; i < affinityArrary.Length; i++)
                affinityArrary[i].HideUI();
        }
    }
}

