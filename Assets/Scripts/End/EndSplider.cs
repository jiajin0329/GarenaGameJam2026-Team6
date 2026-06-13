using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class EndSplider : MonoBehaviour
    {
        [SerializeField]
        private Slider _affinitySlider;

        [SerializeField]
        private TextMeshProUGUI _valueText;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        public void SetAffinity(int _affinity)
        {
            _affinitySlider.DOValue(_affinitySlider.value + _affinity, 1f).SetEase(Ease.Linear);
        }
    }
}
