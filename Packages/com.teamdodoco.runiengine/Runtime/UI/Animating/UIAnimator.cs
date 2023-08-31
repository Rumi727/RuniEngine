#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuniEngine.UI.Animating
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class UIAnimator : UIBase
    {
        public bool isPlaying { get; private set; }
        public bool isReverse { get; set; }

        public float time
        {
            get => _time.Clamp(0);
            set
            {
                if (_time != value)
                {
                    _time = value;
                    LayoutUpdate();
                }
            }
        }
        [SerializeField, Min(0)] float _time = 0;

        public float duration { get; private set; }

        public float length => animations.Count > 0 ? animations.Max(x => x != null ? x.length : 0) : 0;

        public List<UIAnimation?> animations => _animations;
        [SerializeField] List<UIAnimation?> _animations = new List<UIAnimation?>();

        public bool playOnAwake { get => _playOnAwake; set => _playOnAwake = value; }
        [SerializeField] bool _playOnAwake = true;

        public float playOnAwakeDelay { get => _playOnAwakeDelay.Clamp(0); set => _playOnAwakeDelay = value; }
        [SerializeField, Min(0)] float _playOnAwakeDelay = 0;

        protected override async void OnEnable()
        {
            LayoutUpdate();

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                UnityEditor.EditorApplication.update += TimeUpdate;
#endif

            if (Kernel.isPlaying && playOnAwake)
            {
                await UniTask.Delay((int)(playOnAwakeDelay * 1000));
                if (!Kernel.isPlaying)
                    return;

                Play();
            }
        }

#if UNITY_EDITOR
        protected override void OnDisable() => UnityEditor.EditorApplication.update -= TimeUpdate;
#endif


        protected virtual void Update()
        {
            if (Kernel.isPlaying)
                TimeUpdate();
        }

        public void TimeUpdate()
        {
            if (isPlaying)
            {
                if (isReverse)
                    _time -= Kernel.deltaTime;
                else
                    _time += Kernel.deltaTime;

                LayoutUpdate();

                if (isReverse && time <= 0)
                {
                    isReverse = false;

                    _time = 0;
                    Pause();
                }
                else if (time >= duration)
                {
                    _time = duration;
                    Pause();
                }
            }
        }

        public void LayoutUpdate()
        {
            for (int i = 0; i < animations.Count; i++)
            {
                UIAnimation? animation = animations[i];
                if (animation == null)
                    continue;

                animation.Init(this);
                animation.LayoutUpdate();
            }
        }

        public virtual void Play() => Play(length);

        public virtual void Play(float duration)
        {
            _time = 0;
            this.duration = duration;

            isReverse = false;
            isPlaying = true;
        }

        public void Reverse()
        {
            _time = length;
            duration = length;

            isReverse = true;
            isPlaying = true;
        }

        public virtual void Pause()
        {
            isPlaying = false;
            LayoutUpdate();
        }

        public virtual void UnPause() => UnPause(length);

        public virtual void UnPause(float duration)
        {
            isPlaying = true;
            this.duration = duration;
        }

        public virtual void Stop()
        {
            _time = 0;
            duration = 0;

            isPlaying = false;
            LayoutUpdate();
        }
    }
}
