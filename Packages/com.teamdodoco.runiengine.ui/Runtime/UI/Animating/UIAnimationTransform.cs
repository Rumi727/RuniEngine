using UnityEngine;

namespace RuniEngine.UI.Animating
{
    [DisallowMultipleComponent]
    public class UIAnimationTransform : UIAnimation
    {
        public UIAnimationTransformMode mode => _mode;
        [SerializeField] UIAnimationTransformMode _mode = UIAnimationTransformMode.none;

        #region Position
        public AnimationCurve positionX => _positionX;
        [SerializeField] AnimationCurve _positionX = new AnimationCurve();
        public AnimationCurve positionY => _positionY;
        [SerializeField] AnimationCurve _positionY = new AnimationCurve();
        public AnimationCurve positionZ => _positionZ;
        [SerializeField] AnimationCurve _positionZ = new AnimationCurve();
        #endregion

        #region Rotation
        public AnimationCurve rotationX => _rotationX;
        [SerializeField] AnimationCurve _rotationX = new AnimationCurve();
        public AnimationCurve rotationY => _rotationY;
        [SerializeField] AnimationCurve _rotationY = new AnimationCurve();
        public AnimationCurve rotationZ => _rotationZ;
        [SerializeField] AnimationCurve _rotationZ = new AnimationCurve();
        #endregion

        #region Scale
        public AnimationCurve scaleX => _scaleX;
        [SerializeField] AnimationCurve _scaleX = new AnimationCurve();
        public AnimationCurve scaleY => _scaleY;
        [SerializeField] AnimationCurve _scaleY = new AnimationCurve();
        public AnimationCurve scaleZ => _scaleZ;
        [SerializeField] AnimationCurve _scaleZ = new AnimationCurve();
        #endregion

        public override float length
        {
            get
            {
                float max = 0;

                if (mode.HasFlag(UIAnimationTransformMode.positionX))
                    max = max.Max(GetCurveLength(positionX));
                if (mode.HasFlag(UIAnimationTransformMode.positionY))
                    max = max.Max(GetCurveLength(positionY));
                if (mode.HasFlag(UIAnimationTransformMode.positionZ))
                    max = max.Max(GetCurveLength(positionZ));

                if (mode.HasFlag(UIAnimationTransformMode.rotationX))
                    max = max.Max(GetCurveLength(rotationX));
                if (mode.HasFlag(UIAnimationTransformMode.rotationY))
                    max = max.Max(GetCurveLength(rotationY));
                if (mode.HasFlag(UIAnimationTransformMode.rotationZ))
                    max = max.Max(GetCurveLength(rotationZ));

                if (mode.HasFlag(UIAnimationTransformMode.scaleX))
                    max = max.Max(GetCurveLength(scaleX));
                if (mode.HasFlag(UIAnimationTransformMode.scaleY))
                    max = max.Max(GetCurveLength(scaleY));
                if (mode.HasFlag(UIAnimationTransformMode.scaleZ))
                    max = max.Max(GetCurveLength(scaleZ));

                return max;
            }
        }

        public override void LayoutUpdate()
        {
            UIAnimator? animator = this.animator;
            if (animator == null)
                return;

            Vector3 position = transform.localPosition;
            Vector3 rotation = transform.localEulerAngles;
            Vector3 scale = transform.localScale;

            if (mode.HasFlag(UIAnimationTransformMode.positionX))
                position.x = positionX.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.positionY))
                position.y = positionY.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.positionZ))
                position.z = positionZ.Evaluate(animator.time);

            if (mode.HasFlag(UIAnimationTransformMode.rotationX))
                rotation.x = rotationX.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.rotationY))
                rotation.y = rotationY.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.rotationZ))
                rotation.z = rotationZ.Evaluate(animator.time);

            if (mode.HasFlag(UIAnimationTransformMode.scaleX))
                scale.x = scaleX.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.scaleY))
                scale.y = scaleY.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationTransformMode.scaleZ))
                scale.z = scaleZ.Evaluate(animator.time);

            transform.localPosition = position;
            transform.localEulerAngles = rotation;
            transform.localScale = scale;
        }
    }
}
