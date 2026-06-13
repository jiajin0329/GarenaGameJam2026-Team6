using System;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class QuestionService
    {
        private QuestionModel _model;
        private LevelConfig _config;

        private GetQuestionService _getQuestionService;
        private GetWrongAnswerService _getWrongAnswerService;

        public QuestionService(QuestionModel _model, LevelConfig _config)
        {
            this._model = _model;
            this._config = _config;

            _getQuestionService = new(_config);
            _getWrongAnswerService = new(_config);
        }

        public bool CanAskQuestion()
        {
            if (_model.timer < _model.questionInterval)
                return false;

            if (_model.questionCount < 1)
                return false;

            _model.ClearTimer();

            return true;
        }

        public void AskQuestion(QuestionView _view, Action _correctAction, Action _wrongAction)
        {
            Questions _question = _getQuestionService.GetQuestion();
            string _wrongAnswer = _getWrongAnswerService.GetWrongAnswer(_question);

            byte _selectionIndex = (byte)UnityEngine.Random.Range(0, 2);
            int _characterIndex = JudgeCharacterIndex(_question);

            // 左右隨機
            if (_selectionIndex == 0)
                _view.AskQuestion(_question.characterQuestionContext, _characterIndex, _question.rightAnswer, _wrongAnswer, _correctAction, _wrongAction);
            else
                _view.AskQuestion(_question.characterQuestionContext, _characterIndex, _wrongAnswer, _question.rightAnswer, _wrongAction, _correctAction);
        }

        private int JudgeCharacterIndex(Questions _question)
        {
            if (_question.characterName == _config.questionsConfigA.dataArray[0].characterName)
                return 0;
            else if (_question.characterName == _config.questionsConfigB.dataArray[0].characterName)
                return 1;
            else
                return 2;
        }

        public bool CheckAnswer(string _answer, string _correctAnswer)
        {
            if (_answer != _correctAnswer)
                return false;

            return true;
        }

        public bool CanQuestionFinish()
        {
            if (_model.questionCount < 1 && _model.timer >= _model.questionInterval)
                return true;

            return false;
        }
    }
}

