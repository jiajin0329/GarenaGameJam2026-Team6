using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Questions
{
    [HorizontalGroup("Row")]
    [SerializeField]
    private string _characterName;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _characterQuestionType;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _characterQuestionContext;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _rightAnswer;

    public string characterName => _characterName;
    public string characterQuestionType => _characterQuestionType;
    public string characterQuestionContext => _characterQuestionContext;
    public string rightAnswer => _rightAnswer;

    public static class QuestionType
    {
        public const string characterName = "_characterName";
        public const string characterNickName = "_characterNickName";
        public const string birthday = "_birthday";
        public const string starSign = "_starSign";
        public const string likeFood = "_likeFood";
        public const string hateFood = "_hateFood";
        public const string hobby1 = "_hobby1";
        public const string hobby2 = "_hobby2";
    }
}