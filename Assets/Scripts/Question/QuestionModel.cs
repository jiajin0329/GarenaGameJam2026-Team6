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

        public bool isWillNextQuestion { get; private set; }

        private LevelConfig _levelConfig;

        public QuestionModel(LevelConfig _levelConfig)
        {
            this._levelConfig = _levelConfig;
            questionInterval = 60f / _levelConfig.bpm * _levelConfig.questionIntervalBeatAmount;
            timer = questionInterval;
            questionCount = _levelConfig.questionCount;
        }

        public void Tick(float _deltaTime)
        {
            timer += _deltaTime;
        }

        public void AskQuestion()
        {
            questionCount--;
            ClearTimer();
            isLastQuestionNoAnser = true;
        }

        private void ClearTimer()
        {
            timer = 0f;
        }

        public void Answer()
        {
            isLastQuestionNoAnser = false;
        }

        public void WillNextQuestion()
        {
            isWillNextQuestion = true;
        }

        public void NextQuestion()
        {
            timer = questionInterval;
            isWillNextQuestion = false;
        }

        public void LastQuestionNoAnswer()
        {
            isLastQuestionNoAnser = false;
        }

        public void Reset()
        {
            questionCount = _levelConfig.questionCount;
            isLastQuestionNoAnser = false;
        }
    }
}

