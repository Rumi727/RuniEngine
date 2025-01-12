#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.UI.Themes
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class RectTransformStyleSetter : StyleSetterBase
    {
        public RectTransform? targetRectTransform => _targetRectTransform;
        [SerializeField, NotNullField] RectTransform? _targetRectTransform;

        DrivenRectTransformTracker tracker;

        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        public override void Refresh()
        {
            tracker.Clear();
            base.Refresh();
        }

        public override void Refresh(Type? editInScriptType, ThemeStyle style)
        {
            tracker.Clear();

            if (targetRectTransform == null)
                return;

            editInScript = editInScriptType;

            if (!Kernel.isPlaying)
                tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Anchors);

            rectTransform.pivot = style.rectTransform.pivot;

            rectTransform.anchorMin = style.rectTransform.anchorMin;
            rectTransform.anchorMax = style.rectTransform.anchorMax;

            rectTransform.offsetMin = style.rectTransform.offset.min;
            rectTransform.offsetMax = new Vector2(-style.rectTransform.offset.max.x, -style.rectTransform.offset.max.y);
        }
    }
}
