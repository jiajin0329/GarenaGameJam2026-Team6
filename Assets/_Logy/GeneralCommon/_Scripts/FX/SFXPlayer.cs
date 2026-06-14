using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace Logy.UnityCommonV01
{
    [Serializable]
    public class SFXPlayer : Logy.UnityCommon.ProgressV01.Progress
    {
        public static SFXPlayer instance { get; private set; }

        [SerializeField]
        private SFXPlayerSetting _musicSetting;

        [SerializeField]
        private SFXPlayerSetting _soundEffectSetting;

        [SerializeField]
        private SFXPlayerSetting _voiceSetting;

        [SerializeField]
        private AudioSource _playAudioSource;

        [SerializeField]
        private AudioSource _playOneShotAudioSource;

        public float masterVolumeNormalization = 1f;
        public float musicVolumeNormalization = 1f;
        public float soundEffectVolumeNormalization = 1f;
        public float voiceVolumeNormalization = 1f;

        private Dictionary<string, AudioClipData[]> _audioClipDataArrayDictionary = new();

        private Dictionary<string, List<int>> _dictionaryListUnplayedIndex = new();
        private Dictionary<string, int> _dictionaryListAudioClipDataLastIndex = new();
        private Dictionary<string, float> _dictionaryCdTime = new();

        // 為了降低GC而預先分配的 List
        private List<string> _cdKeysCache = new();
        private StringBuilder _key = new();
        private AudioClipData _currentPlayAudioClipData;

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[{typeof(SFXPlayer).Name}] Duplicate instance detected on '{gameObject.name}'. Destroying it.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            AddAudioClipDataToDictionary(_musicSetting);
            AddAudioClipDataToDictionary(_soundEffectSetting);
            AddAudioClipDataToDictionary(_voiceSetting);
        }

        private void AddAudioClipDataToDictionary(SFXPlayerSetting _setting)
        {
            if (_setting == null)
                return;

            foreach (AudioClipSetting _audioClipSetting in _setting.audioClipSettings)
            {
                if (!_audioClipSetting.isEnable)
                    continue;

                _key.Clear();
                _key.Append(_audioClipSetting.name);

                List<AudioClipData> _listAudioClipData = new();
                foreach (AudioClipData _audioClipData in _audioClipSetting.audioClipDatas)
                {
                    if (!_audioClipData.isEnable)
                        continue;

                    _audioClipData.type = _setting.type;

                    _listAudioClipData.Add(_audioClipData);

                    if (_audioClipData.preload)
                        _audioClipData.audioClip.LoadAudioData();
                }

                _audioClipDataArrayDictionary.Add(_key.ToString(), _listAudioClipData.ToArray());
            }
        }

        private void Update()
        {
            CheckCdTime();
            UpdatePlayAudioSourceVolume();
        }

        private void CheckCdTime()
        {
            if (_dictionaryCdTime.Count < 1)
                return;

            // 將暫存 Keys 的 List 清空並重新裝載。
            // AddRange 在底層對 Dictionary.Keys 集合會呼叫 ICollection 介面的 CopyTo，此操作沒有任何 GC 分配。
            // 如此便能避免使用 .ToArray() 在每一幀產生新的 string[]，有效降低 GC (0 allocation)。
            _cdKeysCache.Clear();
            _cdKeysCache.AddRange(_dictionaryCdTime.Keys);

            for (int i = 0; i < _cdKeysCache.Count; i++)
            {
                string _key = _cdKeysCache[i];
                _dictionaryCdTime[_key] -= Time.deltaTime;
                if (_dictionaryCdTime[_key] <= 0)
                    _dictionaryCdTime.Remove(_key);
            }
        }

        private void UpdatePlayAudioSourceVolume()
        {
            if (_currentPlayAudioClipData == null)
                return;

            _playAudioSource.volume = CalculateVolume(_currentPlayAudioClipData);
        }

        public void Play(AudioName _name, float _startTime = 0f, float _fadeDuration = 0f, bool _isLoop = false)
        {
            _key.Clear();
            _key.Append(_name);

            if (!_audioClipDataArrayDictionary.ContainsKey(_key.ToString()))
                return;

            AudioClipData _audioClipData = GetAudioClipData(_name);
            _currentPlayAudioClipData = _audioClipData;

            _playAudioSource.Stop();

            _playAudioSource.clip = _audioClipData.audioClip;
            _playAudioSource.time = _startTime;
            _playAudioSource.loop = _isLoop;

            if (_fadeDuration > 0f)
            {
                _playAudioSource.volume = 0f;
                _playAudioSource.DOFade(CalculateVolume(_audioClipData), _fadeDuration);
            }
            else
                _playAudioSource.volume = CalculateVolume(_audioClipData);

            _playAudioSource.Play();
        }

        private float CalculateVolume(AudioClipData _audioClipData)
        {
            float _volume = _audioClipData.volume * masterVolumeNormalization;

            switch (_audioClipData.type)
            {
                case AudioClipData.Type.music:
                    _volume *= musicVolumeNormalization;
                    break;
                case AudioClipData.Type.soundEffect:
                    _volume *= soundEffectVolumeNormalization;
                    break;
                case AudioClipData.Type.voice:
                    _volume *= voiceVolumeNormalization;
                    break;
            }

            return _volume;
        }

        public void Stop() => _playAudioSource.Stop();

        public void StopWhitFadeVolume(float _duration = 1f)
        {
            _playAudioSource.DOFade(0f, _duration).OnComplete(() => _playAudioSource.Stop());
        }

        public void PlayOneShot(AudioName _name)
        {
            StringBuilder _key = new();
            _key.Append(_name);

            if (!_audioClipDataArrayDictionary.ContainsKey(_key.ToString()))
                return;

            AudioClipData _audioClipData = GetAudioClipData(_name);

            _playOneShotAudioSource.PlayOneShot(_audioClipData.audioClip, CalculateVolume(_audioClipData));

#if UNITY_EDITOR
            Debug.Log($"PlayOneShot: {_name}");
#endif
        }

        private AudioClipData GetAudioClipData(AudioName _name)
        {
            var _key = _name.ToString();

            if (!_dictionaryListUnplayedIndex.ContainsKey(_key))
                _dictionaryListUnplayedIndex.Add(_key, CreateIndexList(_audioClipDataArrayDictionary[_key].Length));
            else if (_dictionaryListUnplayedIndex[_key].Count < 1)
                _dictionaryListUnplayedIndex[_key] = CreateIndexList(_audioClipDataArrayDictionary[_key].Length);

            List<int> _listUnplayedIndex = _dictionaryListUnplayedIndex[_key];
            int _listUnplayedIndex_Index = GetListUnplayedIndex_Index(_key);
            int _audioClipDataIndex = _listUnplayedIndex[_listUnplayedIndex_Index];
            AudioClipData _audioClipData = _audioClipDataArrayDictionary[_key][_audioClipDataIndex];

            if (_dictionaryListAudioClipDataLastIndex.ContainsKey(_key))
                _dictionaryListAudioClipDataLastIndex[_key] = _audioClipDataIndex;
            else
                _dictionaryListAudioClipDataLastIndex.Add(_key, _audioClipDataIndex);

            _listUnplayedIndex.RemoveAt(_listUnplayedIndex_Index);

            return _audioClipData;
        }

        private List<int> CreateIndexList(int _count)
        {
            List<int> _list = new List<int>();
            for (int i = 0; i < _count; i++)
                _list.Add(i);
            return _list;
        }

        private int GetListUnplayedIndex_Index(string _key)
        {
            List<int> _listUnplayedIndex = _dictionaryListUnplayedIndex[_key];

            int _listIndex = UnityEngine.Random.Range(0, _listUnplayedIndex.Count);
            int _index = _listUnplayedIndex[_listIndex];

            if (_dictionaryListAudioClipDataLastIndex.ContainsKey(_key))
            {
                // 如果取得的index和上次播放的index相同，則更換index
                if (_index == _dictionaryListAudioClipDataLastIndex[_key])
                {
                    _listIndex++;
                    _listIndex = _listIndex == _listUnplayedIndex.Count ? _listIndex - _listUnplayedIndex.Count : _listIndex;
                }
            }

            return _listIndex;
        }

        public void PlayOneShotWhenAudioClipEnd(AudioName _name, Action<AudioClip> _playEvent = null, bool _isNotMute = true)
        {
            _key.Clear();
            _key.Append(_name);

            if (!_audioClipDataArrayDictionary.ContainsKey(_key.ToString()))
                return;

            if (_dictionaryCdTime.ContainsKey(_key.ToString()))
                return;

            AudioClipData _audioClipData = GetAudioClipData(_name);

            if (_isNotMute)
            {
                _playOneShotAudioSource.PlayOneShot(_audioClipData.audioClip, CalculateVolume(_audioClipData));
            }

            _dictionaryCdTime.Add(_key.ToString(), _audioClipData.audioClip.length);
            _playEvent?.Invoke(_audioClipData.audioClip);
        }

        public void Destroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}