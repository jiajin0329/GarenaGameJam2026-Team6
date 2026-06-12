using System.Collections.Generic;
using Logy.UnityCommon;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class QuestionService
    {
        private QuestionModel _model;

        private GoogleSheetDataGetterQuestions _questionConfig;
        private GoogleSheetDataGetterWrongAnswers _wrongAnswerConfig;

        private List<Questions> _questionsList;

        private List<string> _wrongCharacterNameList;
        private List<string> _wrongCharacterNickNameList;
        private List<string> _wrongBirthdaysList;
        private List<string> _wrongStarSignList;
        private List<string> _wrongLikeFoodList;
        private List<string> _wrongHateFoodList;
        private List<string> _wrongHobbie1List;
        private List<string> _wrongHobbie2List;

        public QuestionService(QuestionModel _model, GoogleSheetDataGetterQuestions _questions, GoogleSheetDataGetterWrongAnswers _wrongAnswers)
        {
            this._model = _model;
            this._questionConfig = _questions;
            this._wrongAnswerConfig = _wrongAnswers;

            BuildQuestionsList();
            BuildWrongAnswersList();
        }

        private void BuildQuestionsList()
        {
            _questionsList = new();

            for (int i = 0; i < _questionConfig.dataArray.Length; i++)
            {
                _questionsList.Add(_questionConfig.dataArray[i]);
            }
        }

        private void BuildWrongAnswersList()
        {
            _wrongCharacterNameList = new();
            _wrongCharacterNickNameList = new();
            _wrongBirthdaysList = new();
            _wrongStarSignList = new();
            _wrongLikeFoodList = new();
            _wrongHateFoodList = new();
            _wrongHobbie1List = new();
            _wrongHobbie2List = new();

            for (int i = 0; i < _wrongAnswerConfig.dataArray.Length; i++)
            {
                _wrongCharacterNameList.Add(_wrongAnswerConfig.dataArray[i].characterName);
                _wrongCharacterNickNameList.Add(_wrongAnswerConfig.dataArray[i].characterNickName);
                _wrongBirthdaysList.Add(_wrongAnswerConfig.dataArray[i].birthday);
                _wrongStarSignList.Add(_wrongAnswerConfig.dataArray[i].starSign);
                _wrongLikeFoodList.Add(_wrongAnswerConfig.dataArray[i].likeFood);
                _wrongHateFoodList.Add(_wrongAnswerConfig.dataArray[i].hateFood);
                _wrongHobbie1List.Add(_wrongAnswerConfig.dataArray[i].hobby1);
                _wrongHobbie2List.Add(_wrongAnswerConfig.dataArray[i].hobby2);
            }
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

        public void InstantiateQuestion(QuestionView _view)
        {
            Questions _question = GetQuestion();
            string _wrongAnswer = GetWrongAnswer(_question);
            _view.Instantiate(_question.characterQuestionContext, _question.rightAnswer, _wrongAnswer);
        }

        private Questions GetQuestion()
        {
            int _questionIndex = Random.Range(0, _questionsList.Count);
            Questions _question = _questionsList[_questionIndex];
            _questionsList.RemoveAt(_questionIndex);

            return _question;
        }

        private string GetWrongAnswer(Questions _question)
        {
            string _type = _question.characterQuestionType;

            switch (_type)
            {
                case Questions.QuestionType.characterName:
                    return GetWrongAnswerFormList(_type, _wrongCharacterNameList);
                case Questions.QuestionType.characterNickName:
                    return GetWrongAnswerFormList(_type, _wrongCharacterNickNameList);
                case Questions.QuestionType.birthday:
                    return GetWrongAnswerFormList(_type, _wrongBirthdaysList);
                case Questions.QuestionType.starSign:
                    return GetWrongAnswerFormList(_type, _wrongStarSignList);
                case Questions.QuestionType.likeFood:
                    return GetWrongAnswerFormList(_type, _wrongLikeFoodList);
                case Questions.QuestionType.hateFood:
                    return GetWrongAnswerFormList(_type, _wrongHateFoodList);
                case Questions.QuestionType.hobby1:
                    return GetWrongAnswerFormList(_type, _wrongHobbie1List);
                case Questions.QuestionType.hobby2:
                    return GetWrongAnswerFormList(_type, _wrongHobbie2List);
                default:
                    return "";
            }
        }

        private string GetWrongAnswerFormList(string _type, List<string> _wrongAnswerList)
        {
            int _wrongAnswerIndex = Random.Range(0, _wrongAnswerList.Count);
            string _wrongAnswer = _wrongAnswerList[_wrongAnswerIndex];
            _wrongAnswerList.RemoveAt(_wrongAnswerIndex);

            if (_wrongAnswerList.Count == 0)
            {
                JudgeResetWrongAnswerList(_type, _wrongAnswerList);
            }

            return _wrongAnswer;
        }

        private void JudgeResetWrongAnswerList(string _type, List<string> _wrongAnswerList)
        {
            switch (_type)
            {
                case Questions.QuestionType.characterName:
                    ResetWrongAnswerList(_type, _wrongCharacterNameList);
                    break;
                case Questions.QuestionType.characterNickName:
                    ResetWrongAnswerList(_type, _wrongCharacterNickNameList);
                    break;
                case Questions.QuestionType.birthday:
                    ResetWrongAnswerList(_type, _wrongBirthdaysList);
                    break;
                case Questions.QuestionType.starSign:
                    ResetWrongAnswerList(_type, _wrongStarSignList);
                    break;
                case Questions.QuestionType.likeFood:
                    ResetWrongAnswerList(_type, _wrongLikeFoodList);
                    break;
                case Questions.QuestionType.hateFood:
                    ResetWrongAnswerList(_type, _wrongHateFoodList);
                    break;
                case Questions.QuestionType.hobby1:
                    ResetWrongAnswerList(_type, _wrongHobbie1List);
                    break;
                case Questions.QuestionType.hobby2:
                    ResetWrongAnswerList(_type, _wrongHobbie2List);
                    break;
            }
        }

        private void ResetWrongAnswerList(string _type, List<string> _wrongAnswerList)
        {
            for (int i = 0; i < _wrongAnswerConfig.dataArray.Length; i++)
            {
                switch (_type)
                {
                    case Questions.QuestionType.characterName:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].characterName);
                        break;
                    case Questions.QuestionType.characterNickName:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].characterNickName);
                        break;
                    case Questions.QuestionType.birthday:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].birthday);
                        break;
                    case Questions.QuestionType.starSign:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].starSign);
                        break;
                    case Questions.QuestionType.likeFood:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].likeFood);
                        break;
                    case Questions.QuestionType.hateFood:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].hateFood);
                        break;
                    case Questions.QuestionType.hobby1:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].hobby1);
                        break;
                    case Questions.QuestionType.hobby2:
                        _wrongAnswerList.Add(_wrongAnswerConfig.dataArray[i].hobby2);
                        break;
                }
            }
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

