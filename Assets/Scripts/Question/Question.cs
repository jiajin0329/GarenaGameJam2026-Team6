using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Question
    {
        [SerializeField]
        private QuestionModel _model;

        private QuestionService _service;

        private Action _correctAnswerEvent;
        public void AddCorrectAnswerListener(Action _action) => _correctAnswerEvent += _action;
        public void RemoveCorrectAnswerListener(Action _action) => _correctAnswerEvent -= _action;

        private Action _wrongAnswerEvent;
        public void AddWrongAnswerListener(Action _action) => _wrongAnswerEvent += _action;
        public void RemoveWrongAnswerListener(Action _action) => _wrongAnswerEvent -= _action;

        private Action _lastQuestionNoAnswerEvent;
        public void AddLastQuestionNoAnswerListener(Action _action) => _lastQuestionNoAnswerEvent += _action;
        public void RemoveLastQuestionNoAnswerListener(Action _action) => _lastQuestionNoAnswerEvent -= _action;

        private Action _questionFinsihEvent;
        public void AddQuestionFinsihListener(Action _action) => _questionFinsihEvent += _action;
        public void RemoveQuestionFinsihListener(Action _action) => _questionFinsihEvent -= _action;

        public Question(LevelConfig _levelConfig)
        {
            _model = new(_levelConfig.questionCount, _levelConfig.bpm, _levelConfig.questionIntervalBeatAmount);
            _service = new(_model);
        }

        public void Tick(float _deltaTime)
        {
            _model.Tick(_deltaTime);
        }

        public void TryQuestion()
        {
            if (!_service.CanQuestion())
                return;

            TryLastQuestionNoAnswer();

            _model.Question();
            Debug.Log(nameof(_service.CanQuestion));
        }

        private void TryLastQuestionNoAnswer()
        {
            if (!_model.isLastQuestionNoAnser)
                return;

            _model.LastQuestionNoAnswer();
            _lastQuestionNoAnswerEvent?.Invoke();
            Debug.Log(nameof(TryLastQuestionNoAnswer));
        }

        public void Answer(string _answer, string _correctAnswer)
        {
            _model.Answer();
            Debug.Log(nameof(Answer));

            if (_service.CheckAnswer(_answer, _correctAnswer))
            {
                _correctAnswerEvent?.Invoke();
            }
            else
            {
                _wrongAnswerEvent?.Invoke();
            }
        }

        public void TryQuestionFinish()
        {
            if (!_service.CanQuestionFinish())
                return;

            TryLastQuestionNoAnswer();

            _questionFinsihEvent?.Invoke();
            Debug.Log(nameof(_service.CanQuestionFinish));
        }
    }
}

