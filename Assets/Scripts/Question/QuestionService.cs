namespace GarenaGameJam2026Team6
{
    public class QuestionService
    {
        private QuestionModel _model;

        public QuestionService(QuestionModel _model)
        {
            this._model = _model;
        }

        public bool CanQuestion()
        {
            if (_model.timer < _model.questionInterval)
                return false;

            if (_model.questionCount < 1)
                return false;

            _model.ClearTimer();

            return true;
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

