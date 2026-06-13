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

        public void AskQuestion(string _questionText, int _characterIndex, string _selection1Text, string _selection2Text, Action _onAnswerA, Action _onAnswerB)
        {
            _characterViewSwitcher.PlayDialogue(_questionText, _characterIndex, _selection1Text, _selection2Text);

            switch (_characterIndex)
            {
                case 0:
                    _characterViewSwitcher.AddAnswerA1Listener(_onAnswerA);
                    _characterViewSwitcher.AddAnswerB1Listener(_onAnswerB);
                    break;
                case 1:
                    _characterViewSwitcher.AddAnswerA2Listener(_onAnswerA);
                    _characterViewSwitcher.AddAnswerB2Listener(_onAnswerB);
                    break;
                case 2:
                    _characterViewSwitcher.AddAnswerA3Listener(_onAnswerA);
                    _characterViewSwitcher.AddAnswerB3Listener(_onAnswerB);
                    break;
            }
        }
    }
}

