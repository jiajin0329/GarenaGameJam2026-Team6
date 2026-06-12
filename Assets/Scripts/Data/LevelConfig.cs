using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "ScriptableObject/" + nameof(LevelConfig))]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField]
        public int bpm { get; private set; } = 120;

        [field: SerializeField]
        public int oneTimeBeatCount { get; private set; } = 4;
    }
}
