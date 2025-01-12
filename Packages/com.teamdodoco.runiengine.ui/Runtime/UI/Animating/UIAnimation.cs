#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Animating
{
    public abstract class UIAnimation : UIBase
    {
        public UIAnimator? animator
        {
            get
            {
                if (_animator != null && !_animator.animations.Contains(this))
                    _animator = null;

                return _animator;
            }
            private set => _animator = value;
        }
        UIAnimator? _animator;

        public abstract float length { get; }

        public virtual void Init(UIAnimator? animator) => this.animator = animator;

        public abstract void LayoutUpdate();

        public float GetCurveLength(AnimationCurve curve) => curve.length > 0 ? curve.keys[curve.length - 1].time : 0;

    }
}
