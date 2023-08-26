#nullable enable
using RuniEngine.UI;
using UnityEngine;

namespace RuniEngine
{
    public abstract class FitterBase : UIBase
    {
        protected DrivenRectTransformTracker tracker;

        protected override void OnEnable() => SetDirty();

        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        protected override void OnRectTransformDimensionsChange() => SetDirty();

        protected override void OnTransformParentChanged() => SetDirty();

        protected override void OnDidApplyAnimationProperties() => SetDirty();

#if UNITY_EDITOR
        protected override void OnValidate() => onDirty = true;

        bool onDirty = false;
        protected virtual void Update()
        {
            if (onDirty)
                SetDirty();
        }
#endif

        public abstract void SetDirty();
    }
}
