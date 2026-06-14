using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class QuestionService
    {
        private QuestionModel _model;
        private LevelConfig _config;
        private AffinityManager _affinityManager;

        [SerializeField]
        private ChatacterShowLogic _chatacterShowLogic;

        private GetQuestionService _getQuestionService;
        private GetWrongAnswerService _getWrongAnswerService;

        public Action<float> calculateRemainingTimeEvent;

        public void Initialize(QuestionModel _model, LevelConfig _config, AffinityManager _affinityManager)
        {
            this._model = _model;
            this._config = _config;
            this._affinityManager = _affinityManager;

            _getQuestionService = new(_config);
            _getWrongAnswerService = new(_config);
        }

        public bool CanAskQuestion()
        {
            if (_model.isWillNextQuestion)
                return false;

            if (_model.timer < _model.questionInterval)
                return false;

            if (_model.questionCount < 1)
                return false;

            return true;
        }

        public void AskQuestion(QuestionView _view, Action _correctAction, Action _wrongAction)
        {
            Questions _question = _getQuestionService.GetQuestion();
            string _wrongAnswer = _getWrongAnswerService.GetWrongAnswer(_question);

            byte _selectionIndex = (byte)UnityEngine.Random.Range(0, 2);
            int _characterIndex = JudgeCharacterIndex(_question);

            _correctAction += () => CorrentSetSaveFileBool(_characterIndex, _question.characterQuestionType);

            _correctAction += () => _affinityManager.Change(_characterIndex, _config.addAffinity);
            _wrongAction += () => _affinityManager.Change(_characterIndex, -_config.subAffinity);

            _affinityManager.ShowUI(_characterIndex);

            Debug.Log("rightAnswer: " + _question.rightAnswer);

            // 左右隨機
            if (_selectionIndex == 0)
                _view.AskQuestion(_question.characterQuestionContext, _characterIndex, _question.rightAnswer, _wrongAnswer, _correctAction, _wrongAction);
            else
                _view.AskQuestion(_question.characterQuestionContext, _characterIndex, _wrongAnswer, _question.rightAnswer, _wrongAction, _correctAction);
        }

        private void CorrentSetSaveFileBool(int _characterIndex, string _questionType)
        {
            if (_characterIndex == 0)
            {
                if (_questionType == Questions.QuestionType.characterName)
                    SaveSystem.SaveFile_instance.isKnow_characterName_A = true;
                else if (_questionType == Questions.QuestionType.characterNickName)
                    SaveSystem.SaveFile_instance.isKnow_characterNickName_A = true;
                else if (_questionType == Questions.QuestionType.birthday)
                    SaveSystem.SaveFile_instance.isKnow_birthday_A = true;
                else if (_questionType == Questions.QuestionType.starSign)
                    SaveSystem.SaveFile_instance.isKnow_starSign_A = true;
                else if (_questionType == Questions.QuestionType.likeFood)
                    SaveSystem.SaveFile_instance.isKnow_likeFood_A = true;
                else if (_questionType == Questions.QuestionType.hateFood)
                    SaveSystem.SaveFile_instance.isKnow_hateFood_A = true;
                else if (_questionType == Questions.QuestionType.hobby1)
                    SaveSystem.SaveFile_instance.isKnow_hobby1_A = true;
                else if (_questionType == Questions.QuestionType.hobby2)
                    SaveSystem.SaveFile_instance.isKnow_hobby2_A = true;
            }
            else if (_characterIndex == 1)
            {
                if (_questionType == Questions.QuestionType.characterName)
                    SaveSystem.SaveFile_instance.isKnow_characterName_B = true;
                else if (_questionType == Questions.QuestionType.characterNickName)
                    SaveSystem.SaveFile_instance.isKnow_characterNickName_B = true;
                else if (_questionType == Questions.QuestionType.birthday)
                    SaveSystem.SaveFile_instance.isKnow_birthday_B = true;
                else if (_questionType == Questions.QuestionType.starSign)
                    SaveSystem.SaveFile_instance.isKnow_starSign_B = true;
                else if (_questionType == Questions.QuestionType.likeFood)
                    SaveSystem.SaveFile_instance.isKnow_likeFood_B = true;
                else if (_questionType == Questions.QuestionType.hateFood)
                    SaveSystem.SaveFile_instance.isKnow_hateFood_B = true;
                else if (_questionType == Questions.QuestionType.hobby1)
                    SaveSystem.SaveFile_instance.isKnow_hobby1_B = true;
                else if (_questionType == Questions.QuestionType.hobby2)
                    SaveSystem.SaveFile_instance.isKnow_hobby2_B = true;
            }
            else if (_characterIndex == 2)
            {
                if (_questionType == Questions.QuestionType.characterName)
                    SaveSystem.SaveFile_instance.isKnow_characterName_C = true;
                else if (_questionType == Questions.QuestionType.characterNickName)
                    SaveSystem.SaveFile_instance.isKnow_characterNickName_C = true;
                else if (_questionType == Questions.QuestionType.birthday)
                    SaveSystem.SaveFile_instance.isKnow_birthday_C = true;
                else if (_questionType == Questions.QuestionType.starSign)
                    SaveSystem.SaveFile_instance.isKnow_starSign_C = true;
                else if (_questionType == Questions.QuestionType.likeFood)
                    SaveSystem.SaveFile_instance.isKnow_likeFood_C = true;
                else if (_questionType == Questions.QuestionType.hateFood)
                    SaveSystem.SaveFile_instance.isKnow_hateFood_C = true;
                else if (_questionType == Questions.QuestionType.hobby1)
                    SaveSystem.SaveFile_instance.isKnow_hobby1_C = true;
                else if (_questionType == Questions.QuestionType.hobby2)
                    SaveSystem.SaveFile_instance.isKnow_hobby2_C = true;
            }

            SaveSystem.SaveSF();
            _chatacterShowLogic.LoadCharacterInfomation(_characterIndex);

            Debug.Log(nameof(CorrentSetSaveFileBool));
        }

        private int JudgeCharacterIndex(Questions _question)
        {
            if (_question.characterName == CharacterEnum.CharacterA.ToString())
                return 0;
            else if (_question.characterName == CharacterEnum.CharacterB.ToString())
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

        public void CalculateRemainingTime(QuestionModel _model)
        {
            float _remainingTime = _model.questionInterval - _model.timer;

            calculateRemainingTimeEvent?.Invoke(_remainingTime);
            Debug.Log(nameof(CalculateRemainingTime));
        }
    }
}

