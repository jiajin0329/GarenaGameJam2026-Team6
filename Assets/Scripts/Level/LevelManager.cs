using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class LevelManager : Logy.UnityCommon.ProgressV01.Progress
    {
        [field: SerializeField]
        public LevelConfig _config { get; private set; }

        [SerializeField]
        private bool _callQuestion = false;

        [field: SerializeField]
        public Beat beat { get; private set; }

        [field: SerializeField]
        public Question question { get; private set; }

        private bool _isEnable = true;

        public override void Initialize()
        {
            beat = new(_config);
            question = new(_config);

            beat.AddBeatListener(question.TryQuestion);
            beat.AddOneTimeBeatListener(question.TryQuestion);

            beat.AddBeatListener(question.TryQuestionFinish);
            beat.AddOneTimeBeatListener(question.TryQuestionFinish);

            question.AddQuestionFinsihListener(() => _isEnable = false);
        }

        private void Update()
        {
            if (!_isEnable)
                return;

            beat.Tick(Time.deltaTime);
            question.Tick(Time.deltaTime);

            if (_callQuestion)
            {
                question.AnswerCorrect();
                _callQuestion = false;
            }
        }
    }
}
