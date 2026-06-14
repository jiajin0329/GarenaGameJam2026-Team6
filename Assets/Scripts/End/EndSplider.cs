using System;
using Cysharp.Threading.Tasks;
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
        private CharacterEnum _characterEnum;

        [SerializeField]
        private Slider _affinitySlider;

        [SerializeField]
        private TextMeshProUGUI _valueText;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private LevelConfig _levelConfig;

        private float _currentAffinity;

        public void Initialize()
        {
            _nameText.text = _characterEnum.ToString();
            _affinitySlider.value = 0;
            _affinitySlider.maxValue = _levelConfig.affinityMax;
            _valueText.text = "0";
        }

        public async UniTask SetAffinity(float _newAffinity)
        {
            transform.DOScale(1.2f, 0.25f).SetEase(Ease.Linear);

            while (_currentAffinity < _newAffinity)
            {
                _currentAffinity += Time.deltaTime * 2f;
                _affinitySlider.value = _currentAffinity;
                _valueText.text = _currentAffinity.ToString("F2");
                await UniTask.Yield();
            }

            transform.DOScale(1f, 0.25f).SetEase(Ease.Linear);

            _affinitySlider.value = _newAffinity;
            _valueText.text = _newAffinity.ToString("F2");
            _currentAffinity = _newAffinity;
        }
    }
}
