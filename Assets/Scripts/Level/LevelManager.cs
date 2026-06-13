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

        public override void Initialize()
        {
            beat = new(_config);

            affinityArrary = new Affinity[3];
            affinityArrary[0] = new(0f, _config.affinityMax);
            affinityArrary[1] = new(0f, _config.affinityMax);
            affinityArrary[2] = new(0f, _config.affinityMax);

            question.Initialize(_config, affinityArrary);

            _timer = new(_config);

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

            _deltaTime = Time.deltaTime;

            beat.Tick(_deltaTime);
            question.Tick(_deltaTime);
            _timer.Tick(_deltaTime);
        }
    }
}
