using System;
using UnityEngine;

namespace Logy.UnityCommonV01
{
    [Serializable]
    public class AudioClipSetting
    {
        [field: SerializeField]
        public bool isEnable { get; private set; } = true;

        [field: SerializeField]
        public AudioName name { get; private set; }

        [field: SerializeField]
        public AudioClipData[] audioClipDatas { get; private set; }
    }

    [Serializable]
    public class AudioClipData
    {
        [field: SerializeField]
        public bool isEnable { get; private set; }

        [field: SerializeField]
        public bool preload { get; private set; }

        [field: SerializeField]
        public AudioClip audioClip { get; private set; }

        [field: SerializeField]
        public float volume { get; private set; }

        [NonSerialized]
        public Type type;

        public enum Type : byte
        {
            music,
            soundEffect,
            voice
        }

        public AudioClipData(bool _isEnable)
        {
            isEnable = _isEnable;
            audioClip = null;
            volume = 1f;
        }
    }

    public enum AudioName : byte
    {
        /// <summary>
        /// 節拍音效
        /// </summary>
        beat,

        /// <summary>
        /// 一節拍音效
        /// </summary>
        oneTimeBeat,

        /// <summary>
        /// 
        /// </summary>
        menuClick,

        /// <summary>
        /// 
        /// </summary>
        mangaClick,

        /// <summary>
        /// 
        /// </summary>
        poba,

        /// <summary>
        /// 
        /// </summary>
        loadTeach,

        /// <summary>
        /// 
        /// </summary>
        po,

        /// <summary>
        /// 
        /// </summary>
        catchEf,

        /// <summary>
        /// 
        /// </summary>
        diaOutA,

        /// <summary>
        /// 
        /// </summary>
        diaOutB,

        /// <summary>
        /// 
        /// </summary>
        diaOutC,

        /// <summary>
        /// 
        /// </summary>
        badFeedback,

        /// <summary>
        /// 
        /// </summary>
        goodFeedback,

        /// <summary>
        /// 
        /// </summary>
        resultOpen,

        /// <summary>
        /// 
        /// </summary>
        resultScoreUp,

        /// <summary>
        /// 
        /// </summary>
        resultScoreDown,


        /// <summary>
        /// 
        /// </summary>
        Menu,

        /// <summary>
        /// 
        /// </summary>
        Game,

        /// <summary>
        /// 
        /// </summary>
        NormalEnd,

        /// <summary>
        /// 
        /// </summary>
        Lost


    }
}