using System;
using UnityEngine;
using UnityEngine.UI;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class End
    {
        public EndModel model { get; private set; }
        private EndService _service;

        [SerializeField]
        private EndView _view;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Button _nextDayButton;

        [SerializeField]
        private Button _endButton;

        private Action _nextDayEvent;
        public void AddNextDayListener(Action _listener) => _nextDayEvent += _listener;
        public void RemoveNextDayListener(Action _listener) => _nextDayEvent -= _listener;

        public void Initialize(Affinity[] _affinityArrary)
        {
            model = new();
            _service = new(model, _affinityArrary);

            _nextDayButton.onClick.AddListener(NextDay);
            _endButton.onClick.AddListener(EndJudge);
            HideUI();
        }

        public void NextDay()
        {
            _nextDayEvent?.Invoke();
            _canvasGroup.gameObject.SetActive(false);
        }

        private void HideUI()
        {
            _canvasGroup.gameObject.SetActive(false);
        }

        public void TryEnd()
        {
            model.LevelFinish();

            if (_service.CanJudeEnd())
            {
                _canvasGroup.gameObject.SetActive(true);
                _nextDayButton.gameObject.SetActive(false);
                _endButton.gameObject.SetActive(true);
                return;
            }
            else
            {
                _canvasGroup.gameObject.SetActive(true);
                _nextDayButton.gameObject.SetActive(true);
                _endButton.gameObject.SetActive(false);
            }
        }

        private void EndJudge()
        {
            if (_service.CanAllCharacterEnd())
            {
                Debug.Log("End");
            }

            else if (_service.CanCharacterAEnd())
            {
                Debug.Log("Character A End");
            }

            else if (_service.CanCharacterBEnd())
            {
                Debug.Log("Character B End");
            }

            else if (_service.CanCharacterCEnd())
            {
                Debug.Log("Character C End");
            }
            else
            {
                Debug.Log("Bad End");
            }
        }
    }
}
