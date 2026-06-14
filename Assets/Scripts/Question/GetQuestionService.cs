using System.Collections.Generic;

namespace GarenaGameJam2026Team6
{
    public class GetQuestionService
    {
        private LevelConfig _config;

        private List<Questions> _questionsList;

        public GetQuestionService(LevelConfig _config)
        {
            this._config = _config;

            BuildQuestionsList();
        }

        private void BuildQuestionsList()
        {
            _questionsList = new();

            int i;
            for (i = 0; i < _config.questionsConfigA.dataArray.Length; i++)
            {
                _questionsList.Add(_config.questionsConfigA.dataArray[i]);
            }
        }

        public Questions GetQuestion()
        {
            int _questionIndex = UnityEngine.Random.Range(0, _questionsList.Count);
            Questions _question = _questionsList[_questionIndex];
            _questionsList.RemoveAt(_questionIndex);

            ResetQuestionsList();

            return _question;
        }

        private void ResetQuestionsList()
        {
            if (_questionsList.Count < 1)
                BuildQuestionsList();
        }
    }
}

