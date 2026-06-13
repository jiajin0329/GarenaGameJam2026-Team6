using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class Question
    {
        [SerializeField]
        private QuestionModel _model;

        [SerializeField]
        private QuestionView _view;

        [SerializeField]
        private QuestionService _service;

        private Action _answerCorrectEvent;
        public void AddAnswerCorrectListener(Action _listener) => _answerCorrectEvent += _listener;
        public void RemoveAnswerCorrectListener(Action _listener) => _answerCorrectEvent -= _listener;

        private Action _answerWrongEvent;
        public void AddAnswerWrongListener(Action _listener) => _answerWrongEvent += _listener;
        public void RemoveAnswerWrongListener(Action _listener) => _answerWrongEvent -= _listener;

        private Action _lastQuestionNoAnswerEvent;
        public void AddLastQuestionNoAnswerListener(Action _listener) => _lastQuestionNoAnswerEvent += _listener;
        public void RemoveLastQuestionNoAnswerListener(Action _listener) => _lastQuestionNoAnswerEvent -= _listener;

        private Action _questionFinsihEvent;
        public void AddQuestionFinsihListener(Action _listener) => _questionFinsihEvent += _listener;
        public void RemoveQuestionFinsihListener(Action _listener) => _questionFinsihEvent -= _listener;

        public void Initialize(LevelConfig _levelConfig)
        {
            _model = new(_levelConfig.questionCount, _levelConfig.bpm, _levelConfig.questionIntervalBeatAmount);
            _service = new(_model, _levelConfig);

            _view.Initialize();
        }

        public void Tick(float _deltaTime)
        {
            _model.Tick(_deltaTime);
        }

        public void TryAskQuestion()
        {
            if (!_service.CanAskQuestion())
                return;

            TryLastQuestionNoAnswer();

            _model.AskQuestion();
            _service.AskQuestion(_view, AnswerCorrect, AnswerWrong);

            Debug.Log(nameof(_service.CanAskQuestion));
        }

        private void TryLastQuestionNoAnswer()
        {
            if (!_model.isLastQuestionNoAnser)
                return;

            _model.LastQuestionNoAnswer();
            _lastQuestionNoAnswerEvent?.Invoke();
            Debug.Log(nameof(TryLastQuestionNoAnswer));
        }

        public void AnswerCorrect()
        {
            _model.Answer();
            _answerCorrectEvent?.Invoke();
            Debug.Log(nameof(AnswerCorrect));
        }

        public void AnswerWrong()
        {
            _model.Answer();
            _answerWrongEvent?.Invoke();
            Debug.Log(nameof(AnswerWrong));
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

