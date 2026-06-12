using System;
using UnityEngine;

namespace Logy.UnityCommonV01
{
    [Serializable]
    public class AudioClipPreloader : MonoBehaviour
    {

        [SerializeField]
        private AudioClip[] _audioClipArray;

        private void Awake()
        {
            foreach (var _audioClip in _audioClipArray)
            {
                if (_audioClip.loadState == AudioDataLoadState.Unloaded)
                    _audioClip.LoadAudioData();
            }
        }
    }
}