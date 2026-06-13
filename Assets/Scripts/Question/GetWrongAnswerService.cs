using System.Collections.Generic;

namespace GarenaGameJam2026Team6
{
    public class GetWrongAnswerService
    {
        private LevelConfig _config;

        private List<string> _wrongCharacterNameList;
        private List<string> _wrongCharacterNickNameList;
        private List<string> _wrongBirthdaysList;
        private List<string> _wrongStarSignList;
        private List<string> _wrongLikeFoodList;
        private List<string> _wrongHateFoodList;
        private List<string> _wrongHobbie1List;
        private List<string> _wrongHobbie2List;

        public GetWrongAnswerService(LevelConfig _config)
        {
            this._config = _config;

            BuildWrongAnswersList();
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

            for (int i = 0; i < _config.wrongAnswerConfig.dataArray.Length; i++)
            {
                AddDataIntoList(_wrongCharacterNameList, _config.wrongAnswerConfig.dataArray[i].characterName);
                AddDataIntoList(_wrongCharacterNickNameList, _config.wrongAnswerConfig.dataArray[i].characterNickName);
                AddDataIntoList(_wrongBirthdaysList, _config.wrongAnswerConfig.dataArray[i].birthday);
                AddDataIntoList(_wrongStarSignList, _config.wrongAnswerConfig.dataArray[i].starSign);
                AddDataIntoList(_wrongLikeFoodList, _config.wrongAnswerConfig.dataArray[i].likeFood);
                AddDataIntoList(_wrongHateFoodList, _config.wrongAnswerConfig.dataArray[i].hateFood);
                AddDataIntoList(_wrongHobbie1List, _config.wrongAnswerConfig.dataArray[i].hobby1);
                AddDataIntoList(_wrongHobbie2List, _config.wrongAnswerConfig.dataArray[i].hobby2);
            }
        }

        private void AddDataIntoList(List<string> _list, string _data)
        {
            if (string.IsNullOrEmpty(_data))
                return;

            _list.Add(_data);
        }

        public string GetWrongAnswer(Questions _question)
        {
            string _type = _question.characterQuestionType;

            switch (_type)
            {
                case Questions.QuestionType.characterName:
                    return GetWrongAnswerFormType(_type, _wrongCharacterNameList);
                case Questions.QuestionType.characterNickName:
                    return GetWrongAnswerFormType(_type, _wrongCharacterNickNameList);
                case Questions.QuestionType.birthday:
                    return GetWrongAnswerFormType(_type, _wrongBirthdaysList);
                case Questions.QuestionType.starSign:
                    return GetWrongAnswerFormType(_type, _wrongStarSignList);
                case Questions.QuestionType.likeFood:
                    return GetWrongAnswerFormType(_type, _wrongLikeFoodList);
                case Questions.QuestionType.hateFood:
                    return GetWrongAnswerFormType(_type, _wrongHateFoodList);
                case Questions.QuestionType.hobby1:
                    return GetWrongAnswerFormType(_type, _wrongHobbie1List);
                case Questions.QuestionType.hobby2:
                    return GetWrongAnswerFormType(_type, _wrongHobbie2List);
                default:
                    return "";
            }
        }

        private string GetWrongAnswerFormType(string _type, List<string> _wrongAnswerList)
        {
            int _wrongAnswerIndex = UnityEngine.Random.Range(0, _wrongAnswerList.Count);
            string _wrongAnswer = _wrongAnswerList[_wrongAnswerIndex];
            _wrongAnswerList.RemoveAt(_wrongAnswerIndex);

            JudgeResetWrongAnswerList(_type, _wrongAnswerList);

            return _wrongAnswer;
        }

        private void JudgeResetWrongAnswerList(string _type, List<string> _wrongAnswerList)
        {
            if (_wrongAnswerList.Count > 0)
                return;

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
            for (int i = 0; i < _config.wrongAnswerConfig.dataArray.Length; i++)
            {
                switch (_type)
                {
                    case Questions.QuestionType.characterName:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].characterName);
                        break;
                    case Questions.QuestionType.characterNickName:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].characterNickName);
                        break;
                    case Questions.QuestionType.birthday:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].birthday);
                        break;
                    case Questions.QuestionType.starSign:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].starSign);
                        break;
                    case Questions.QuestionType.likeFood:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].likeFood);
                        break;
                    case Questions.QuestionType.hateFood:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].hateFood);
                        break;
                    case Questions.QuestionType.hobby1:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].hobby1);
                        break;
                    case Questions.QuestionType.hobby2:
                        _wrongAnswerList.Add(_config.wrongAnswerConfig.dataArray[i].hobby2);
                        break;
                }
            }
        }
    }
}

