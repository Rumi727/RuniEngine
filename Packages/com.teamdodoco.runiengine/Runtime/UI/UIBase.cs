#nullable enable
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuniEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBase : UIBehaviour, IUI
    {
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

        public Graphic? graphic => _graphic = this.GetComponentFieldSave(_graphic, GetComponentMode.none);
        Graphic? _graphic;



        public RectCorner localCorners
        {
            get
            {
                return rectTransform.rect;
            }
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
    }
}
