using System;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [Serializable]
    public class QuestionModel
    {
        [field: SerializeField]
        public float questionInterval { get; private set; }

        [field: SerializeField]
        public float timer { get; private set; }

        [field: SerializeField]
        public int questionCount { get; private set; }

        [field: SerializeField]
        public bool isLastQuestionNoAnser { get; private set; }

        public QuestionModel(int _questionCount, int _bpm, int _questionIntervalBeatAmount)
        {
            questionInterval = 60f / _bpm * _questionIntervalBeatAmount;
            timer = questionInterval;
            questionCount = _questionCount;
        }

        public void Tick(float _deltaTime)
        {
            timer += _deltaTime;
        }

        public void ClearTimer()
        {
            timer -= questionInterval;
        }

        public void AskQuestion()
        {
            questionCount--;
            isLastQuestionNoAnser = true;
        }

        public void Answer()
        {
            isLastQuestionNoAnser = false;
        }

        public void NextQuestion()
        {
            timer = questionInterval;
        }

        public void LastQuestionNoAnswer()
        {
            isLastQuestionNoAnser = false;
        }
    }
}

