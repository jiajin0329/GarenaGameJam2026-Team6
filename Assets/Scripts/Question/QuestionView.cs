using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class QuestionView
    {
        [SerializeField]
        private RectTransform _canvasRectTransform;

        [SerializeField]
        private CharacterViewSwitcher _characterViewSwitcher;

        public void Initialize()
        {
            _characterViewSwitcher = GameObject.Instantiate(_characterViewSwitcher, _canvasRectTransform);
        }

        public void AskQuestion(string _questionText, string _selection1Text, string _selection2Text)
        {
            _characterViewSwitcher.PlayDialogue(_questionText, 1, _selection1Text, _selection2Text);
        }
    }
}

