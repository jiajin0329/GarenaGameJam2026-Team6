using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class LevelManager : Logy.UnityCommon.ProgressV01.Progress
    {
        [field: SerializeField]
        public LevelConfig _config { get; private set; }

        [field: Space(10)]

        [field: SerializeField]
        public Beat beat { get; private set; }

        [field: Space(10)]

        [field: SerializeField]
        public Question question { get; private set; }

        [field: Space(10)]

        [field: SerializeField]
        public AffinityManager affinityManager { get; private set; }

        [field: Space(10)]

        [field: SerializeField]
        public Timer _timer { get; private set; }

        [field: Space(10)]

        [field: SerializeField]
        public End end { get; private set; }

        private bool _isEnable = false;
        private float _deltaTime = 0f;

        private CancellationToken _destroyCancellationToken;

        public override void Initialize()
        {
            beat.Initialize(_config);

            _destroyCancellationToken = destroyCancellationToken;
            question.Initialize(_config, affinityManager, _destroyCancellationToken);

            affinityManager.Initialize(_config);

            _timer.Initialize(_config, end);

            end.Initialize(affinityManager);

            beat.AddBeatListener(question.TryAskQuestion);
            beat.AddOneTimeBeatListener(question.TryAskQuestion);

            beat.AddBeatListener(question.TryQuestionFinish);
            beat.AddOneTimeBeatListener(question.TryQuestionFinish);

            question.AddAskQuestionListener(beat.ResetBeat);

            question.AddQuestionFinsihListener(IsEnableFalse);
            question.AddQuestionFinsihListener(end.TryEnd);

            question.AddCalculateRemainingTimeListener(_timer.GetRemainingTime);

            end.AddNextDayListener(Restart);

            StartGame().Forget();
        }

        private void IsEnableFalse()
        {
            _isEnable = false;
            _timer.IsEnableFalse();
        }

        private async UniTask StartGame()
        {
            await UniTask.Delay((int)(_config.startGameDelay * 1000f), cancellationToken: _destroyCancellationToken);
            _isEnable = true;
        }

        private void Update()
        {
            if (!_isEnable)
                return;

            _deltaTime = Time.deltaTime;

            beat.Tick(_deltaTime);
            _timer.Tick(_deltaTime);
            question.Tick(_deltaTime);
        }

        private void Restart()
        {
            beat.Reset();
            question.Reset();
            _timer.Reset();

            StartGame().Forget();
        }
    }
}
