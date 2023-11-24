#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Animating
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIAnimationAlpha : UIAnimation
    {
        public CanvasGroup? canvasGroup => _canvasGroup = this.GetComponentFieldSave(_canvasGroup);
        CanvasGroup? _canvasGroup;

        public AnimationCurve alpha => _alpha;
        [SerializeField] AnimationCurve _alpha = new AnimationCurve();

        public override float length => GetCurveLength(alpha);

        public override void LayoutUpdate()
        {
            UIAnimator? animator = this.animator;
            if (animator == null || canvasGroup == null)
                return;

            canvasGroup.alpha = alpha.Evaluate(animator.time);
        }
    }
}
