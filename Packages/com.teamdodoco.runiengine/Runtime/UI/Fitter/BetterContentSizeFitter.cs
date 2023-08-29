#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI.Fitter
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class BetterContentSizeFitter : FitterBase, ILayoutSelfController
    {
        [SerializeField] RectTransform? _targetRectTransform; public RectTransform? target { get => _targetRectTransform; set => _targetRectTransform = value; }

        [SerializeField] bool _xSize = false; public bool xSize { get => _xSize; set => _xSize = value; }
        [SerializeField] bool _ySize = false; public bool ySize { get => _ySize; set => _ySize = value; }



        [SerializeField] Vector2 _offset = Vector2.zero; public Vector2 offset { get => _offset; set => _offset = value; }

        [SerializeField, Min(0)] Vector2 _minSize = Vector2.zero; public Vector2 min { get => _minSize; set => _minSize = value; }
        [SerializeField, Min(0)] Vector2 _maxSize = Vector2.zero; public Vector2 max { get => _maxSize; set => _maxSize = value; }



        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        public void HandleSelfFittingAlongAxis(int axis)
        {
            RectTransform target = this.target != null ? this.target : rectTransform;
            if (axis == 0)
            {
                if (!xSize)
                    return;

                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);

                float size = LayoutUtility.GetPreferredSize(target, axis);
                if (max.x <= 0)
                    size = size.Clamp(min.x);
                else
                    size = size.Clamp(min.x, max.x);

                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size + offset.x);
            }
            else
            {
                if (!ySize)
                    return;

                if (!Kernel.isPlaying)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);

                float size = LayoutUtility.GetPreferredSize(target, axis);
                if (max.y <= 0)
                    size = size.Clamp(min.y);
                else
                    size = size.Clamp(min.y, max.y);

                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size + offset.y);
            }
        }

        public void SetLayoutHorizontal()
        {
            tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public void SetLayoutVertical() => HandleSelfFittingAlongAxis(1);

        public override void SetDirty()
        {
            if (!isActiveAndEnabled)
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }


#if UNITY_EDITOR
        protected override void OnValidate() => SetDirty();
#endif
    }
}
