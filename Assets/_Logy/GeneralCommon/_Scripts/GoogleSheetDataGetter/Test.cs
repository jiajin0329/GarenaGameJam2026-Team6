using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Test
{
    [HorizontalGroup("Row")]
    [SerializeField]
    private string _name;

    [HorizontalGroup("Row")]
    [SerializeField]
    private string _description;

    [HorizontalGroup("Row")]
    [SerializeField]
    private float _value;

    [HorizontalGroup("Row")]
    [SerializeField]
    private float _value2;

    public string name => _name;
    public string description => _description;
    public float value => _value;
    public float value2 => _value2;
}