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
    }
}
