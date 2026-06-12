using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Logy.UnityCommonV01
{
    [Serializable]
    public class SoundSetting
    {
        [SerializeField]
        private Slider _masterVolumeSlider;

        [SerializeField]
        private Slider _musicSlider;

        [SerializeField]
        private Slider _soundEffectSlider;

        [SerializeField]
        private Slider _voiceSlider;

        public void Initialize()
        {
            float _masterVolumeNormalization = PlayerPrefs.GetFloat("masterVolumeNormalization", 1f);
            SFXPlayer.instance.masterVolumeNormalization = _masterVolumeNormalization;
            _masterVolumeSlider.value = SFXPlayer.instance.masterVolumeNormalization;

            float _musicVolumeNormalization = PlayerPrefs.GetFloat("musicVolumeNormalization", 1f);
            SFXPlayer.instance.musicVolumeNormalization = _musicVolumeNormalization;
            _musicSlider.value = SFXPlayer.instance.musicVolumeNormalization;

            float _soundEffectVolumeNormalization = PlayerPrefs.GetFloat("soundEffectVolumeNormalization", 1f);
            SFXPlayer.instance.soundEffectVolumeNormalization = _soundEffectVolumeNormalization;
            _soundEffectSlider.value = SFXPlayer.instance.soundEffectVolumeNormalization;

            float _voiceVolumeNormalization = PlayerPrefs.GetFloat("voiceVolumeNormalization", 1f);
            SFXPlayer.instance.voiceVolumeNormalization = _voiceVolumeNormalization;
            _voiceSlider.value = SFXPlayer.instance.voiceVolumeNormalization;

            _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolumeNormalization);
            _musicSlider.onValueChanged.AddListener(SetMusicVolumeNormalization);
            _soundEffectSlider.onValueChanged.AddListener(SetSoundEffectVolumeNormalization);
            _voiceSlider.onValueChanged.AddListener(SetVoiceVolumeNormalization);

            AddPointerUpListener(_masterVolumeSlider, SaveMasterVolumeNormalization);
            AddPointerUpListener(_musicSlider, SaveMusicVolumeNormalization);
            AddPointerUpListener(_soundEffectSlider, SaveSoundEffectVolumeNormalization);
            AddPointerUpListener(_voiceSlider, SaveVoiceVolumeNormalization);
        }

        private void AddPointerUpListener(Slider slider, Action action)
        {
            EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = slider.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entry.callback.AddListener((data) => action?.Invoke());
            trigger.triggers.Add(entry);
        }

        private void SetMasterVolumeNormalization(float _set) => SFXPlayer.instance.masterVolumeNormalization = _set;
        private void SaveMasterVolumeNormalization() => PlayerPrefs.SetFloat("masterVolumeNormalization", SFXPlayer.instance.masterVolumeNormalization);
        private void SetMusicVolumeNormalization(float _set) => SFXPlayer.instance.musicVolumeNormalization = _set;
        private void SaveMusicVolumeNormalization() => PlayerPrefs.SetFloat("musicVolumeNormalization", SFXPlayer.instance.musicVolumeNormalization);
        private void SetSoundEffectVolumeNormalization(float _set) => SFXPlayer.instance.soundEffectVolumeNormalization = _set;
        private void SaveSoundEffectVolumeNormalization() => PlayerPrefs.SetFloat("soundEffectVolumeNormalization", SFXPlayer.instance.soundEffectVolumeNormalization);
        private void SetVoiceVolumeNormalization(float _set) => SFXPlayer.instance.voiceVolumeNormalization = _set;
        private void SaveVoiceVolumeNormalization() => PlayerPrefs.SetFloat("voiceVolumeNormalization", SFXPlayer.instance.voiceVolumeNormalization);


    }
}