#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.Rhythms
{
    [RequireComponent(typeof(AudioSource))]
    public class RhythmableAudioSource : MonoBehaviour, IRhythmable
    {
        public AudioSource? audioSource => _audioSource = this.GetComponentFieldSave(_audioSource);
        [SerializeField, HideInInspector] AudioSource? _audioSource;

        /// <summary>오디오의 현재 시간을 보간하여 반환합니다</summary>
        public double time
        {
            get
            {
                if (audioSource == null)
                    return 0;

                float time = audioSource.time;
                if (time != lastTime)
                {
                    interpolation = 0;
                    lastTime = time;
                }

                return time + interpolation;
            }
            set
            {
                if (audioSource == null)
                    return;

                float result = (float)value;

                audioSource.time = result;
                lastTime = result;
            }
        }
        public double rhythmOffset => _rhythmOffset;
        public double _rhythmOffset;

        BeatBPMPairList IRhythmable.bpms => _bpms ??= new BeatBPMPairList(bpm);
        BeatBPMPairList? _bpms;

        public BPM bpm
        {
            get => _bpm;
            set
            {
                _bpm = value;
                _bpms = new BeatBPMPairList(value);
            }
        }
        [SerializeField] BPM _bpm = new BPM(60);


        [NonSerialized] double interpolation = 0;
        [NonSerialized] float lastTime = 0;

        void Update()
        {
            if (audioSource == null)
                return;

            if (audioSource.isPlaying)
                interpolation += Kernel.unscaledDeltaTimeDouble;
            else
                interpolation = 0;
        }
    }
}
