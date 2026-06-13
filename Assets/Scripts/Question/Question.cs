using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private CancellationToken _cancellationToken;

        private Action _askQuestionEvent;
        public void AddAskQuestionListener(Action _listener) => _askQuestionEvent += _listener;
        public void RemoveAskQuestionListener(Action _listener) => _askQuestionEvent -= _listener;

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

        public void AddCalculateRemainingTimeListener(Action<float> _listener) => _service.calculateRemainingTimeEvent += _listener;
        public void RemoveCalculateRemainingTimeListener(Action<float> _listener) => _service.calculateRemainingTimeEvent -= _listener;

        public void Initialize(LevelConfig _levelConfig, Affinity[] _affinityArrary, CancellationToken _cancellationToken)
        {
            _model = new(_levelConfig);
            _service = new(_model, _levelConfig, _affinityArrary);
            this._cancellationToken = _cancellationToken;

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
            _askQuestionEvent?.Invoke();

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

        private void AnswerCorrect()
        {
            //要在_model.Answer()前執行，因為_model.Answer()會重置timer
            _service.CalculateRemainingTime(_model);
            _model.Answer();
            _answerCorrectEvent?.Invoke();
            Debug.Log(nameof(AnswerCorrect));

            WaitSomeTimeAndNextQuestion(1500).Forget();
        }

        private async UniTaskVoid WaitSomeTimeAndNextQuestion(int _timeMS)
        {
            await UniTask.Delay(_timeMS, cancellationToken: _cancellationToken);
            _model.NextQuestion();
        }

        private void AnswerWrong()
        {
            //要在_model.Answer()前執行，因為_model.Answer()會重置timer
            _service.CalculateRemainingTime(_model);
            _model.Answer();
            _answerWrongEvent?.Invoke();
            Debug.Log(nameof(AnswerWrong));

            WaitSomeTimeAndNextQuestion(1500).Forget();
        }

        public void TryQuestionFinish()
        {
            if (!_service.CanQuestionFinish())
                return;

            TryLastQuestionNoAnswer();

            _questionFinsihEvent?.Invoke();
            Debug.Log(nameof(_service.CanQuestionFinish));
        }

        public void Reset()
        {
            _model.Reset();
        }
    }
}

