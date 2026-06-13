using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class LevelManager : Logy.UnityCommon.ProgressV01.Progress
    {
        [field: SerializeField]
        public LevelConfig _config { get; private set; }

        [field: SerializeField]
        public Beat beat { get; private set; }

        [field: SerializeField]
        public Question question { get; private set; }

        [SerializeField]
        private bool _callQuestion = false;

        private bool _isEnable = false;

        public override void Initialize()
        {
            beat = new(_config);
            question.Initialize(_config);

            beat.AddBeatListener(question.TryAskQuestion);
            beat.AddOneTimeBeatListener(question.TryAskQuestion);

            beat.AddBeatListener(question.TryQuestionFinish);
            beat.AddOneTimeBeatListener(question.TryQuestionFinish);

            question.AddQuestionFinsihListener(() => _isEnable = false);

            StartGame().Forget();
        }

        private async UniTask StartGame()
        {
            await UniTask.Delay((int)(_config.startGameDelay * 1000f), cancellationToken: destroyCancellationToken);
            _isEnable = true;
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
