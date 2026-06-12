using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class WrongAnswers
{
    [HorizontalGroup("Row")]
    [SerializeField]
    private string _characterName;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _characterNickName;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _birthday;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _starSign;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _likeFood;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _hateFood;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _hobby1;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _hobby2;



    public string characterName => _characterName;
    public string characterNickName => _characterNickName;
    public string birthday => _birthday;
    public string starSign => _starSign;
    public string likeFood => _likeFood;
    public string hateFood => _hateFood;
    public string hobby1 => _hobby1;
    public string hobby2 => _hobby2;
}