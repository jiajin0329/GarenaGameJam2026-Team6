using Logy.UnityCommonV01;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public void Initialize(AffinityManager _affinityManager)
        {
            model = new();
            _view.Initialize(_affinityManager.affinityArrary);
            _service = new(model, _affinityManager.affinityArrary);

            _nextDayButton.onClick.AddListener(NextDay);
            _endButton.onClick.AddListener(EndJudge);
            HideUI();
        }

        public void NextDay()
        {
            _nextDayEvent?.Invoke();
            _canvasGroup.gameObject.SetActive(false);
            SFXPlayer.instance.PlayOneShot(AudioName.poba);

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
                _view.UpdateAffinity().Forget();
                SFXPlayer.instance.PlayOneShot(AudioName.resultOpen);
                return;
            }
            else
            {
                _canvasGroup.gameObject.SetActive(true);
                _nextDayButton.gameObject.SetActive(true);
                _endButton.gameObject.SetActive(false);
                _view.UpdateAffinity().Forget();
            }
        }

        private void EndJudge()
        {
            if (_service.CanAllCharacterEnd())
            {
                LoadEndCG.endType = LoadEndCG.EndType.AllCharacterEnd;
                Debug.Log("All Character End");
            }

            else if (_service.CanCharacterAEnd())
            {
                LoadEndCG.endType = LoadEndCG.EndType.CharacterAEnd;
                Debug.Log("Character A End");
            }

            else if (_service.CanCharacterBEnd())
            {
                LoadEndCG.endType = LoadEndCG.EndType.CharacterBEnd;
                Debug.Log("Character B End");
            }

            else if (_service.CanCharacterCEnd())
            {
                LoadEndCG.endType = LoadEndCG.EndType.CharacterCEnd;
                Debug.Log("Character C End");
            }
            else
            {
                LoadEndCG.endType = LoadEndCG.EndType.BadEnd;
                Debug.Log("Bad End");
            }

            SceneManager.LoadScene("EndScene");
        }
    }
}
