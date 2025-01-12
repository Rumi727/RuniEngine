#nullable enable
using UnityEngine;

namespace RuniEngine.UI
{
    public abstract class UIAniBase : UIBase, IUIAni
    {
        public bool disableLerpAni { get => _disableLerpAni; set => _disableLerpAni = value; }
        [SerializeField] bool _disableLerpAni = false;

        public bool useCustomLerpSpeed { get => _useCustomLerpSpeed; set => _useCustomLerpSpeed = value; }
        [SerializeField] bool _useCustomLerpSpeed = false;

        public float lerpSpeed { get => _lerpSpeed; set => _lerpSpeed = value; }

        [SerializeField, Range(0, 1)] float _lerpSpeed = 0.05f;

        public float currentLerpSpeed
        {
            get
            {
                if (!disableLerpAni && Kernel.isPlaying)
                {
                    if (useCustomLerpSpeed)
                        return lerpSpeed * Kernel.fpsUnscaledSmoothDeltaTime;
                    else
                        return UIManager.UserData.defaultLerpAniSpeed * Kernel.fpsUnscaledSmoothDeltaTime;
                }
                else
                    return 1;
            }
        }

        /// <summary>
        /// Please put <see cref="OnEnable"/> when overriding
        /// </summary>
        protected override void OnEnable()
        {
            bool temp = disableLerpAni;
            disableLerpAni = true;

            SetDirty();

            disableLerpAni = temp;
        }

        /// <summary>
        /// Please put <see cref="OnRectTransformDimensionsChange"/> when overriding
        /// </summary>
        protected override void OnRectTransformDimensionsChange() => SetDirty();

        /// <summary>
        /// Please put <see cref="OnTransformParentChanged"/> when overriding
        /// </summary>
        protected override void OnTransformParentChanged() => SetDirty();

        /// <summary>
        /// Please put <see cref="OnDidApplyAnimationProperties"/> when overriding
        /// </summary>
        protected override void OnDidApplyAnimationProperties() => SetDirty();

#if UNITY_EDITOR
        /// <summary>
        /// Please put <see cref="OnValidate"/> when overriding<code></code>Override only in UNITY_EDITOR state
        /// </summary>
        protected override void OnValidate() => onDirty = true;

        bool onDirty = false;
        /// <summary>
        /// Please put <see cref="Update"/> when overriding
        /// </summary>
        protected virtual void Update()
        {
            if (onDirty)
                SetDirty();

            if (!disableLerpAni || !Kernel.isPlaying)
                LayoutUpdate();
        }
#else
        /// <summary>
        /// Please put <see cref="Update"/> when overriding
        /// </summary>
        protected virtual void Update()
        {
            if (!disableLerpAni)
                LayoutUpdate();
        }
#endif

        bool isRefresh = false;
        /// <summary>
        /// Put <see cref="SetDirty"/> at the bottom when overriding
        /// </summary>
        public virtual void SetDirty()
        {
            if ((disableLerpAni || Kernel.isPlaying) && !isRefresh)
            {
                try
                {
                    isRefresh = true;
                    LayoutUpdate();
                }
                finally
                {
                    isRefresh = false;
                }
            }
        }

        public abstract void LayoutUpdate();
    }
}
