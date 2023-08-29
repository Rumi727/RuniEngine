#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace RuniEngine.UI.Fitter
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ContentAspectRatioFitter : AspectRatioFitter, IFitter
    {
        #region IUI
        public Canvas? canvas => _canvas = this.GetComponentInParentFieldSave(_canvas, true);
        Canvas? _canvas;

        public RectTransform? parentRectTransform
        {
            get
            {
                if (_parentRectTransform == null || _parentRectTransform.gameObject != transform.parent.gameObject)
                    _parentRectTransform = transform.parent as RectTransform;

                return _parentRectTransform;
            }
        }
        RectTransform? _parentRectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null || _rectTransform.gameObject != gameObject)
                {
                    if (transform is RectTransform rectTransform)
                        _rectTransform = rectTransform;
                    else
                        _rectTransform = gameObject.AddComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        RectTransform? _rectTransform;

#pragma warning disable CS8603 // 가능한 null 참조 반환입니다.
        public RectTransformTool rectTransformTool => _rectTransformTool = this.GetComponentFieldSave(_rectTransformTool);
#pragma warning restore CS8603 // 가능한 null 참조 반환입니다.
        RectTransformTool? _rectTransformTool;

        public Graphic? graphic => _graphic = this.GetComponentFieldSave(_graphic, GetComponentMode.none);
        Graphic? _graphic;



        public RectCorner localCorners
        {
            get => rectTransform.rect;
            set
            {
                Rect rect = value;

                rectTransform.offsetMin = rect.min;
                rectTransform.offsetMax = rect.max;
            }
        }

        public RectCorner worldCorners
        {
            get
            {
                rectTransform.GetWorldCorners(worldCornersArray);
                return new RectCorner(worldCornersArray[0], worldCornersArray[1], worldCornersArray[2], worldCornersArray[3]);
            }
            set
            {
                Rect rect = value;
                Matrix4x4 matrix4x = rectTransform.worldToLocalMatrix;

                rectTransform.offsetMin = matrix4x.MultiplyPoint(rect.min);
                rectTransform.offsetMax = matrix4x.MultiplyPoint(rect.max);
            }
        }
        readonly Vector3[] worldCornersArray = new Vector3[4];
        #endregion

        public RectTransform? targetRectTransform => _targetRectTransform;
        [SerializeField] RectTransform? _targetRectTransform;

        protected override void Update()
        {
            base.Update();
            SetDirty();
        }

        /// <summary>
        /// Put <see cref="SetDirty"/> at the bottom when overriding
        /// </summary>
        public new virtual void SetDirty()
        {
            base.SetDirty();
            RectTransform? targetRectTransform = this.targetRectTransform != null ? this.targetRectTransform : rectTransform;

            float xSize = LayoutUtility.GetPreferredSize(targetRectTransform, 0);
            float ySize = LayoutUtility.GetPreferredSize(targetRectTransform, 1);

            float aspectRatio = xSize / ySize;
            if (!float.IsNormal(aspectRatio))
                this.aspectRatio = aspectRatio;
            else
                this.aspectRatio = 1;
        }
    }
}
