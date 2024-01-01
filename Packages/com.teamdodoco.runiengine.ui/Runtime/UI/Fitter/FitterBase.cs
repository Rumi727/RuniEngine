#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Fitter
{
    public abstract class FitterBase : UIBase, IFitter
    {
        protected DrivenRectTransformTracker tracker;

        /// <summary>
        /// Please put <see cref="OnEnable"/> when overriding
        /// </summary>
        protected override void OnEnable() => SetDirty();

        /// <summary>
        /// Please put <see cref="OnDestroy"/> when overriding
        /// </summary>
        protected override void OnDestroy()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
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
        /// Please put <see cref="OnValidate"/> when overriding<code></code>Override only in UNITY_EDITOR state
        /// </summary>
        protected virtual void Update()
        {
            if (onDirty)
                SetDirty();
        }
#endif

        public abstract void SetDirty();
    }
}
