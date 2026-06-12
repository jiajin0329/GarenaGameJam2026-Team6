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
    public string characterQuestionContext  => _characterQuestionContext;
    public string rightAnswer => _rightAnswer;
}