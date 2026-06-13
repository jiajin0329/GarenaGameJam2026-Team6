using Logy.UnityCommon;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "ScriptableObject/" + nameof(LevelConfig))]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField]
        public float startGameDelay { get; private set; } = 2f;

        [field: SerializeField]
        public int bpm { get; private set; } = 120;

        [field: SerializeField]
        public int oneTimeBeatAmount { get; private set; } = 4;

        [field: SerializeField]
        public int questionIntervalBeatAmount { get; private set; } = 8;

        [field: SerializeField]
        public int questionCount { get; private set; } = 15;

        [field: SerializeField]
        public GoogleSheetDataGetterQuestions questionsConfigA { get; private set; }

        [field: SerializeField]
        public GoogleSheetDataGetterQuestions questionsConfigB { get; private set; }

        [field: SerializeField]
        public GoogleSheetDataGetterQuestions questionsConfigC { get; private set; }

        [field: SerializeField]
        public GoogleSheetDataGetterWrongAnswers wrongAnswerConfig { get; private set; }
    }
}
