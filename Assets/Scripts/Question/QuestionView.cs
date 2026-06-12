using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class QuestionView
    {
        public void Instantiate(string _questionText, string _selection1Text, string _selection2Text)
        {
            Debug.Log($"Question: {_questionText}, Selection1: {_selection1Text}, Selection2: {_selection2Text}");
        }
    }
}

