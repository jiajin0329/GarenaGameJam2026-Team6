using System.Threading;
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

        [field: SerializeField]
        public Affinity[] affinityArrary { get; private set; }

        [field: SerializeField]
        public Timer _timer { get; private set; }

        [SerializeField]
        private bool _callQuestion = false;

        private bool _isEnable = false;
        private float _deltaTime = 0f;

        private CancellationToken _destroyCancellationToken;

        public override void Initialize()
        {
            beat.Initialize(_config);

            affinityArrary = new Affinity[3];
            affinityArrary[0] = new(0f, _config.affinityMax);
            affinityArrary[1] = new(0f, _config.affinityMax);
            affinityArrary[2] = new(0f, _config.affinityMax);

            _destroyCancellationToken = destroyCancellationToken;
            question.Initialize(_config, affinityArrary, _destroyCancellationToken);

            _timer.Initialize(_config);

            beat.AddBeatListener(question.TryAskQuestion);
            beat.AddOneTimeBeatListener(question.TryAskQuestion);

            beat.AddBeatListener(question.TryQuestionFinish);
            beat.AddOneTimeBeatListener(question.TryQuestionFinish);

            question.AddAskQuestionListener(beat.ResetBeat);
            question.AddQuestionFinsihListener(() => _isEnable = false);

            question.AddCalculateRemainingTimeListener(_timer.GetRemainingTime);

            StartGame().Forget();
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
            question.Tick(_deltaTime);
            _timer.Tick(_deltaTime);
        }
    }
}
