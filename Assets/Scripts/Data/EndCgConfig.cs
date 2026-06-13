using UnityEngine;

namespace GarenaGameJam2026Team6
{
    [CreateAssetMenu(fileName = nameof(EndCgConfig), menuName = "ScriptableObject/" + nameof(EndCgConfig))]
    public class EndCgConfig : ScriptableObject
    {
        [field: SerializeField]
        public Sprite allCharacterEndCg { get; private set; }

        [field: SerializeField]
        public Sprite characterAEndCg { get; private set; }

        [field: SerializeField]
        public Sprite characterBEndCg { get; private set; }

        [field: SerializeField]
        public Sprite characterCEndCg { get; private set; }

        [field: SerializeField]
        public Sprite badEndCg { get; private set; }
    }
}
