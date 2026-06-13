using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GarenaGameJam2026Team6
{
    public class Timer
    {
        private float _timer = 0f;
        private float _finishTime = 0f;
        private float _normalizedCurrentTime = 0f;
        private CharacterViewSwitcher _characterViewSwitcher;

        public Timer(LevelConfig _config, CharacterViewSwitcher _characterViewSwitcher)
        {
            // this._finishTime = _config.finishTime;
            this._characterViewSwitcher = _characterViewSwitcher;
        }

        public void Tick(float _deltaTime)
        {
            _timer += _deltaTime;
        }


        private void UpdateUI()
        {
            _normalizedCurrentTime = _timer / _finishTime;
        }
    }
}
