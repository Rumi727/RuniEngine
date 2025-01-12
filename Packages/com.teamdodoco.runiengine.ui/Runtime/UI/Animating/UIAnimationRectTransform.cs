#nullable enable
using UnityEngine;

namespace RuniEngine.UI.Animating
{
    [DisallowMultipleComponent]
    public class UIAnimationRectTransform : UIAnimation
    {
        DrivenRectTransformTracker tracker;

        public UIAnimationRectTransformMode mode => _mode;
        [SerializeField] UIAnimationRectTransformMode _mode = UIAnimationRectTransformMode.none;

        #region Anchored Position
        public AnimationCurve anchoredPositionX => _anchoredPositionX;
        [SerializeField] AnimationCurve _anchoredPositionX = new AnimationCurve();
        public AnimationCurve anchoredPositionY => _anchoredPositionY;
        [SerializeField] AnimationCurve _anchoredPositionY = new AnimationCurve();
        public AnimationCurve anchoredPositionZ => _anchoredPositionZ;
        [SerializeField] AnimationCurve _anchoredPositionZ = new AnimationCurve();
        #endregion

        #region Size Delta
        public AnimationCurve sizeDeltaX => _sizeDeltaX;
        [SerializeField] AnimationCurve _sizeDeltaX = new AnimationCurve();
        public AnimationCurve sizeDeltaY => _sizeDeltaY;
        [SerializeField] AnimationCurve _sizeDeltaY = new AnimationCurve();
        #endregion

        #region Offset
        public AnimationCurve offsetMinX => _offsetMinX;
        [SerializeField] AnimationCurve _offsetMinX = new AnimationCurve();
        public AnimationCurve offsetMinY => _offsetMinY;
        [SerializeField] AnimationCurve _offsetMinY = new AnimationCurve();

        public AnimationCurve offsetMaxX => _offsetMaxX;
        [SerializeField] AnimationCurve _offsetMaxX = new AnimationCurve();
        public AnimationCurve offsetMaxY => _offsetMaxY;
        [SerializeField] AnimationCurve _offsetMaxY = new AnimationCurve();
        #endregion

        #region Anchor
        public AnimationCurve anchorMinX => _anchorMinX;
        [SerializeField] AnimationCurve _anchorMinX = new AnimationCurve();
        public AnimationCurve anchorMinY => _anchorMinY;
        [SerializeField] AnimationCurve _anchorMinY = new AnimationCurve();

        public AnimationCurve anchorMaxX => _anchorMaxX;
        [SerializeField] AnimationCurve _anchorMaxX = new AnimationCurve();
        public AnimationCurve anchorMaxY => _anchorMaxY;
        [SerializeField] AnimationCurve _anchorMaxY = new AnimationCurve();
        #endregion

        #region Pivot
        public AnimationCurve pivotX => _pivotX;
        [SerializeField] AnimationCurve _pivotX = new AnimationCurve();
        public AnimationCurve pivotY => _pivotY;
        [SerializeField] AnimationCurve _pivotY = new AnimationCurve();
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

                if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX))
                    max = max.Max(GetCurveLength(anchoredPositionX));
                if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY))
                    max = max.Max(GetCurveLength(anchoredPositionY));
                if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ))
                    max = max.Max(GetCurveLength(anchoredPositionZ));

                if (mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX))
                    max = max.Max(GetCurveLength(sizeDeltaX));
                if (mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY))
                    max = max.Max(GetCurveLength(sizeDeltaY));

                if (mode.HasFlag(UIAnimationRectTransformMode.offsetMinX))
                    max = max.Max(GetCurveLength(offsetMinX));
                if (mode.HasFlag(UIAnimationRectTransformMode.offsetMinY))
                    max = max.Max(GetCurveLength(offsetMinY));

                if (mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX))
                    max = max.Max(GetCurveLength(offsetMaxX));
                if (mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY))
                    max = max.Max(GetCurveLength(offsetMaxY));

                if (mode.HasFlag(UIAnimationRectTransformMode.anchorMinX))
                    max = max.Max(GetCurveLength(anchorMinX));
                if (mode.HasFlag(UIAnimationRectTransformMode.anchorMinY))
                    max = max.Max(GetCurveLength(anchorMinY));

                if (mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX))
                    max = max.Max(GetCurveLength(anchorMaxX));
                if (mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY))
                    max = max.Max(GetCurveLength(anchorMaxY));

                if (mode.HasFlag(UIAnimationRectTransformMode.pivotX))
                    max = max.Max(GetCurveLength(pivotX));
                if (mode.HasFlag(UIAnimationRectTransformMode.pivotY))
                    max = max.Max(GetCurveLength(pivotY));

                if (mode.HasFlag(UIAnimationRectTransformMode.rotationX))
                    max = max.Max(GetCurveLength(rotationX));
                if (mode.HasFlag(UIAnimationRectTransformMode.rotationY))
                    max = max.Max(GetCurveLength(rotationY));
                if (mode.HasFlag(UIAnimationRectTransformMode.rotationZ))
                    max = max.Max(GetCurveLength(rotationZ));

                if (mode.HasFlag(UIAnimationRectTransformMode.scaleX))
                    max = max.Max(GetCurveLength(scaleX));
                if (mode.HasFlag(UIAnimationRectTransformMode.scaleY))
                    max = max.Max(GetCurveLength(scaleY));
                if (mode.HasFlag(UIAnimationRectTransformMode.scaleZ))
                    max = max.Max(GetCurveLength(scaleZ));

                return max;
            }
        }

        /// <summary>
        /// Please put <see cref="OnDestroy"/> when overriding
        /// </summary>
        protected override void OnDestroy()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

#if UNITY_EDITOR
        public override void Init(UIAnimator? animator)
        {
            base.Init(animator);

            if (!Kernel.isPlaying)
                tracker.Clear();
        }
#endif

        public override void LayoutUpdate()
        {
            UIAnimator? animator = this.animator;
            if (animator == null)
                return;

            Vector3 anchoredPosition = rectTransform.anchoredPosition;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float? offsetMinX = null;
            float? offsetMinY = null;
            float? offsetMaxX = null;
            float? offsetMaxY = null;
            Vector2 anchorMin = rectTransform.anchorMin;
            Vector2 anchorMax = rectTransform.anchorMax;
            Vector2 pivot = rectTransform.pivot;
            Vector3 rotation = rectTransform.localEulerAngles;
            Vector3 scale = rectTransform.localScale;

            if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionX))
            {
                anchoredPosition.x = anchoredPositionX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionY))
            {
                anchoredPosition.y = anchoredPositionY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.anchoredPositionZ))
            {
                anchoredPosition.z = anchoredPositionZ.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionZ);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaX))
            {
                sizeDelta.x = sizeDeltaX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.sizeDeltaY))
            {
                sizeDelta.y = sizeDeltaY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.offsetMinX))
            {
                offsetMinX = this.offsetMinX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.offsetMinY))
            {
                offsetMinY = this.offsetMinY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.offsetMaxX))
            {
                offsetMaxX = this.offsetMaxX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.offsetMaxY))
            {
                offsetMaxY = this.offsetMaxY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.anchorMinX))
            {
                anchorMin.x = anchorMinX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchorMinX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.anchorMinY))
            {
                anchorMin.y = anchorMinY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchorMinY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.anchorMaxX))
            {
                anchorMax.x = anchorMaxX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchorMaxX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.anchorMaxY))
            {
                anchorMax.y = anchorMaxY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.AnchorMaxY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.pivotX))
            {
                pivot.x = pivotX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.PivotX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.pivotY))
            {
                pivot.y = pivotY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.PivotY);
            }

            if (mode.HasFlag(UIAnimationRectTransformMode.rotation) && !Kernel.isPlaying)
                tracker.Add(this, rectTransform, DrivenTransformProperties.Rotation);

            if (mode.HasFlag(UIAnimationRectTransformMode.rotationX))
                rotation.x = rotationX.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationRectTransformMode.rotationY))
                rotation.y = rotationY.Evaluate(animator.time);
            if (mode.HasFlag(UIAnimationRectTransformMode.rotationZ))
                rotation.z = rotationZ.Evaluate(animator.time);

            if (mode.HasFlag(UIAnimationRectTransformMode.scaleX))
            {
                scale.x = scaleX.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.ScaleX);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.scaleY))
            {
                scale.y = scaleY.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.ScaleY);
            }
            if (mode.HasFlag(UIAnimationRectTransformMode.scaleZ))
            {
                scale.z = scaleZ.Evaluate(animator.time);
                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.ScaleZ);
            }

            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;

            if (offsetMinX != null)
                rectTransform.offsetMin = new Vector2((float)offsetMinX, rectTransform.offsetMin.y);
            if (offsetMinY != null)
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.y, (float)offsetMinY);
            if (offsetMaxX != null)
                rectTransform.offsetMax = new Vector2((float)offsetMaxX, rectTransform.offsetMax.y);
            if (offsetMaxY != null)
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.y, (float)offsetMaxY);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.localEulerAngles = rotation;
            rectTransform.localScale = scale;
        }
    }
}
